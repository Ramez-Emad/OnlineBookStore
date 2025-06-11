using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderHeader? orderHeader = _unitOfWork.GetRepository<OrderHeader>().Get(o => o.Id == id , o => o.ApplicationUser);
            if (orderHeader == null)
            {
                return NotFound();
            }
            var orderitems = _unitOfWork.GetRepository<OrderDetail>().GetAll(o => o.Product).Where(o => o.OrderHeaderId == id).ToList();

            var orderVM = new OrderVM()
            {
                OrderHeader = orderHeader,
                OrderDetail = orderitems
            };

            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderDetail(OrderHeader orderHeader)
        {
            var orderHeaderFromDb = _unitOfWork.GetRepository<OrderHeader>().Get(u => u.Id == orderHeader.Id);

            if (orderHeaderFromDb == null) {
                return NotFound();
            }

            orderHeaderFromDb.Name = orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderHeader.StreetAddress;
            orderHeaderFromDb.City = orderHeader.City;
            orderHeaderFromDb.State = orderHeader.State;
            orderHeaderFromDb.PostalCode = orderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = orderHeader.TrackingNumber;
            }
            _unitOfWork.GetRepository<OrderHeader>().Update(orderHeaderFromDb);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { id = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing(int id)
        {
            (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStatus(id, SD.StatusInProcess);

            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder( OrderHeader orderHeader)
        {

            var orderHeaderDb = _unitOfWork.GetRepository<OrderHeader>().Get(o => o.Id == orderHeader.Id);

            if (orderHeaderDb == null)
                return NotFound();

            orderHeaderDb.TrackingNumber = orderHeader.TrackingNumber;
            orderHeaderDb.Carrier = orderHeader.Carrier;
            orderHeaderDb.OrderStatus = SD.StatusShipped;
            orderHeaderDb.ShippingDate = DateTime.Now;

            if (orderHeaderDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.GetRepository<OrderHeader>().Update(orderHeaderDb);
            await _unitOfWork.SaveChangesAsync();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { id = orderHeaderDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder(int id)
        {

            var orderHeader = _unitOfWork.GetRepository<OrderHeader>().Get(o => o.Id == id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            await _unitOfWork.SaveChangesAsync();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { id =id });

        }



        [HttpPost]
        public async Task<IActionResult> Details_PAY_NOW(int id)
        {
            var orderHeader = _unitOfWork.GetRepository<OrderHeader>().Get(o => o.Id == id);

            var OrderDetails = _unitOfWork.GetRepository<OrderDetail>().GetAll(o => o.Product).Where(o => o.OrderHeaderId == id);

            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStripePaymentID(orderHeader.Id, session.Id, session.PaymentIntentId);

            await _unitOfWork.SaveChangesAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = _unitOfWork.GetRepository<OrderHeader>().Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.SaveChangesAsync();
                }


            }


            return View(orderHeaderId);
        }


        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader?> ordersHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
                ordersHeaders = _unitOfWork.GetRepository<OrderHeader>().GetAll(o => o.ApplicationUser);
            else
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ordersHeaders = _unitOfWork.GetRepository<OrderHeader>().GetAll(o => o.ApplicationUser).Where(o=>o.ApplicationUserId == userId);
            }


            switch (status)
            {
                case "pending":
                    ordersHeaders = ordersHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    ordersHeaders = ordersHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    ordersHeaders = ordersHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    ordersHeaders = ordersHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }

            return Json(new { data = ordersHeaders });
        }

        #endregion
    }
}
