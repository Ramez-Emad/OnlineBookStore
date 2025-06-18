using Bulky.BL.Services._ServicesManager;
using Bulky.Utility;
using BulkyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IServicesManager _servicesManager;

        public OrderController(IServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            OrderHeader orderHeader = await _servicesManager.OrderServices.GetOrderHeaderByIdAsync(id);

            var orderitems = await _servicesManager.OrderServices.GetOrderDetailsByIdAsync(id);

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
            if (ModelState.IsValid)
            {
                await _servicesManager.OrderServices.UpdateOrderHeader(orderHeader);

                TempData["Success"] = "Order Details Updated Successfully.";
            }

            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing(int id)
        {
            await _servicesManager.OrderServices.UpdateStatus(id, SD.StatusInProcess);
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder(OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                await _servicesManager.OrderServices.ShipOrder(orderHeader);

                TempData["Success"] = "Order Shipped Successfully.";
            }

            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder(int id)
        {

            var orderHeader = await  _servicesManager.OrderServices.GetOrderHeaderByIdAsync(id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                await _servicesManager.OrderServices.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                await _servicesManager.OrderServices.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { id = id });

        }



        [HttpPost]
        public async Task<IActionResult> Details_PAY_NOW(int id)
        {
            var orderHeader = await _servicesManager.OrderServices.GetOrderHeaderByIdAsync(id);

            var OrderDetails = await _servicesManager.OrderServices.GetOrderDetailsByIdAsync(id);

            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderHeader.Id}";
            var CancelUrl = domain + $"admin/order/details?orderId={orderHeader.Id}";
            
            var items = OrderDetails.Select(item => new CartItem() { Title = item.Product.Title, Price = item.Price, Quantity = item.Count });

            try
            {
                var session = await _servicesManager.PaymentService.PayOrder(items, orderHeader.Id, SuccessUrl, CancelUrl);

                Response.Headers.Append("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch (StripeException e)
            {
                switch (e.StripeError.Type)
                {
                    case "card_error":
                        Console.WriteLine($"A payment error occurred: {e.StripeError.Message}");
                        break;
                    case "invalid_request_error":
                        Console.WriteLine("An invalid request occurred.");
                        break;
                    default:
                        Console.WriteLine("Another problem occurred, maybe unrelated to Stripe.");
                        break;
                }
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }


        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = await _servicesManager.OrderServices.GetOrderHeaderByIdAsync(orderHeaderId);

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _servicesManager.OrderServices.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);

                    await _servicesManager.OrderServices.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                }
            }


            return View(orderHeaderId);
        }


        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {

            var ordersHeaders = await _servicesManager.OrderServices.GetAllOrdersAsync();

            if (!User.IsInRole(SD.Role_Admin) && !User.IsInRole(SD.Role_Employee))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ordersHeaders = ordersHeaders.Where(o => o.ApplicationUserId == userId);
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
