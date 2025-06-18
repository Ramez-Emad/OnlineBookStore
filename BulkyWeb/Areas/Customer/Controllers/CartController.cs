using Bulky.BL.Models.Products;
using Bulky.BL.Services._ServicesManager;
using Bulky.DataAccess.Entities;
using Bulky.Utility;
using BulkyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IServicesManager _servicesManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(IServicesManager servicesManager, UserManager<ApplicationUser> userManager)
        {
            _servicesManager = servicesManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);

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
            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);

            if (cart != null)
            {
                var product = await _servicesManager.ProductService.GetProductByIdAsync(Id);
                if (product != null)
                    await _servicesManager.CartServices.DecrementProductInUserCart(cart, product);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Plus(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);

            if (cart != null)
            {
                var product = await _servicesManager.ProductService.GetProductByIdAsync(Id);
                if(product != null)
                    await _servicesManager.CartServices.IncrementProductInUserCart(cart, product);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);

            await _servicesManager.CartServices.DeleteProductFromUserCart(cart, Id);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Summary()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId);

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.Items.Any()) return NotFound();

            summary.CartItems = cart.Items;

            if (!ModelState.IsValid)
            {
                return View(summary);
            }

            var IsInRoleCompany = User.IsInRole(SD.Role_Company);

            await _servicesManager.OrderServices.CreateOrder(summary.OrderHeader, cart, IsInRoleCompany);

            if (!IsInRoleCompany)
            {

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={summary.OrderHeader.Id}";
                var CancelUrl = domain + $"customer/cart/index";
               
                try
                {
                    var session = await _servicesManager.PaymentService.PayOrder(summary.CartItems,
                        summary.OrderHeader.Id, SuccessUrl, CancelUrl);

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
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = summary.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {

            var orderHeader = await _servicesManager.OrderServices.GetOrderHeaderByIdAsync(id);

            if (orderHeader == null) return NotFound();


            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _servicesManager.OrderServices.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);

                    await _servicesManager.OrderServices.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);

                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            await _servicesManager.CartServices.DeleteUserCartAsync(orderHeader.ApplicationUserId);
            return View(id);
        }

    }


}
