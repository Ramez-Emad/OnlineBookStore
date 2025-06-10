using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Product = Bulky.Models.Product;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;

        public CartController(IUnitOfWork unitOfWork, ICartRepository cartRepository)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartAsync(userId!);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = userId!,
                    Items = []
                };
            }
            return View(cart);
        }

        public async Task<IActionResult> Minus(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartAsync(userId!);

            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);

                if (item != null)
                {
                    cart.TotalCost -= (item.Price * item.Quantity);

                    if (item.Quantity > 1)
                    {
                        item.Quantity--;

                        var product = _unitOfWork.GetRepository<Product>().Get(prod => prod.Id == item.ProductId);
                        if (product != null)
                            item.Price = GetPrice(product, item.Quantity);

                        cart.TotalCost += (item.Price * item.Quantity);

                    }
                    else
                    {

                        cart.Items.Remove(item);
                        if (cart.Items.Count == 0)
                        {
                            await _cartRepository.DeleteCartAsync(userId!);
                            return RedirectToAction("Index");

                        }
                    }
                }
                await _cartRepository.CreateOrUpdateCartAsync(cart);

            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Plus(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartAsync(userId!);

            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);

                if (item != null)
                {
                    cart.TotalCost -= (item.Price * item.Quantity);

                    item.Quantity++;

                    var product = _unitOfWork.GetRepository<Product>().Get(prod => prod.Id == item.ProductId);
                    if (product != null)
                        item.Price = GetPrice(product, item.Quantity);

                    cart.TotalCost += (item.Price * item.Quantity);

                }
                await _cartRepository.CreateOrUpdateCartAsync(cart);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartAsync(userId!);

            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);

                if (item != null)
                {
                    cart.TotalCost -= (item.Quantity * item.Price);
                    cart.Items.Remove(item);
                    if (cart.Items.Count == 0)
                    {
                        await _cartRepository.DeleteCartAsync(userId!);
                        return RedirectToAction("Index");

                    }
                }
                await _cartRepository.CreateOrUpdateCartAsync(cart);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Summary()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = _unitOfWork.GetRepository<ApplicationUser>().Get(u => u.Id == userId);
            if (user == null) return NotFound();

            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null || !cart.Items.Any()) return NotFound();

            var orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? "",
                PostalCode = user.PostalCode ?? "",
                State = user.State ?? "",
                City = user.City ?? "",
                StreetAddress = user.StreetAddress ?? "",
                OrderTotal = cart.TotalCost
            };

            var summary = new SummaryVM
            {
                OrderHeader = orderHeader,
                CartItems = cart.Items
            };

            return View(summary);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(SummaryVM summary)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null || !cart.Items.Any()) return NotFound();
            summary.CartItems = cart.Items;

            if (!ModelState.IsValid)
            {
                return View(summary);
            }

            ApplicationUser? applicationUser = _unitOfWork.GetRepository<ApplicationUser>().Get(u => u.Id == userId);
            if (applicationUser == null) return NotFound();

            summary.OrderHeader.OrderDate = DateTime.Now;
            summary.OrderHeader.OrderTotal = cart.TotalCost;
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                summary.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                summary.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                summary.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                summary.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.GetRepository<OrderHeader>().Add(summary.OrderHeader);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in summary.CartItems)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = summary.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Quantity
                };
                _unitOfWork.GetRepository<OrderDetail>().Add(orderDetail);
            }
            await _unitOfWork.SaveChangesAsync();

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7179/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={summary.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),       
                    Mode = "payment",
                };
                foreach(var item in cart.Items)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Title
                            }
                        },
                        Quantity = item.Quantity,                   
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                try
                {
                    var service = new SessionService();
                    Session session = service.Create(options);

                    ((IOrderHeaderRepository)_unitOfWork.GetRepository<OrderHeader>()).UpdateStripePaymentID(summary.OrderHeader.Id, session.Id, session.PaymentIntentId);

                    await _unitOfWork.SaveChangesAsync();

                    Response.Headers.Add("Location", session.Url);
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
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = summary.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
           
            OrderHeader orderHeader = _unitOfWork.GetRepository<OrderHeader>().Get(u => u.Id == id);

            ApplicationUser user = _unitOfWork.GetRepository<ApplicationUser>().Get(u => u.Id == orderHeader.ApplicationUserId);

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    (_unitOfWork.GetRepository<OrderHeader>() as IOrderHeaderRepository).UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);

                    await _unitOfWork.SaveChangesAsync();

                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            _cartRepository.DeleteCartAsync(user.Id);
            return View(id);
        }
        private double GetPrice(Product product, int quantity)
        {
            return quantity switch
            {
                >= 100 => product.Price100,
                >= 50 => product.Price50,
                _ => product.Price
            };
        }
    }


}
