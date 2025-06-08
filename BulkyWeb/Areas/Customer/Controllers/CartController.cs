using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;

        public CartController(IUnitOfWork unitOfWork , ICartRepository cartRepository)
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

                    var product =  _unitOfWork.GetRepository<Product>().Get(prod=> prod.Id == item.ProductId);
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

        public IActionResult Summary()
        {
            return View();
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
