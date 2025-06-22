using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.BL.Models.Products;
using Bulky.BL.Services._ServicesManager;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServicesManager _servicesManager;

        public HomeController(ILogger<HomeController> logger, IServicesManager servicesManager)
        {
            _logger = logger;
            _servicesManager = servicesManager;
        }

        public async Task<IActionResult> Index()
        {

            var productList = await _servicesManager.ProductService.GetAllProductsAsync();
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);
                if (cart != null)
                {
                    HttpContext.Session.SetInt32("CartCount", cart.Items.Count());
                }
            }

            return View(productList);
        }

        public async Task<IActionResult> Details(int Id)
        {
            var product = await _servicesManager.ProductService.GetProductByIdAsync(Id);
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(int productId, int quantity)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _servicesManager.CartServices.GetCartByUserIdAsync(userId!);

            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = userId!,
                    Items = []
                };
            }

            var product = await _servicesManager.ProductService.GetProductByIdAsync(productId);

            await _servicesManager.CartServices.CreateOrUpdateUserCartAsync(product!, quantity, cart);

            HttpContext.Session.SetInt32("CartCount", cart.Items.Count());


            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
