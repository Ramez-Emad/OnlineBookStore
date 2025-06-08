using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork , ICartRepository cartRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.GetRepository<Product>().GetAll(p => p.Category);
            return View(productList);
        }

        public IActionResult Details(int Id)
        {
            var product = _unitOfWork.GetRepository<Product>().Get(u => u.Id == Id, p => p.Category);          
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(int productId, int quantity)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepository.GetCartAsync(userId!);
            var product = _unitOfWork.GetRepository<Product>().Get(prod => prod.Id == productId);

            if (product == null)
                return View("Error");

            if (cart == null)
            {
                var item = CreateCartItem(product, quantity);
                cart = new Cart
                {
                    Id = userId!,
                    Items = [item],
                    TotalCost = item.Quantity * item.Price
                };
            }
            else
            {

                cart.Items ??= new List<CartItem>();

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    cart.TotalCost -= existingItem.Price * existingItem.Quantity;
                    existingItem.Quantity += quantity;
                    existingItem.Price = GetPrice(product, existingItem.Quantity);
                    cart.TotalCost += existingItem.Price * existingItem.Quantity;
                }
                else
                {
                    var item = CreateCartItem(product, quantity);
                    cart.Items.Add(item);
                    cart.TotalCost += item.Price * item.Quantity;
                }
            }
            await _cartRepository.CreateOrUpdateCartAsync(cart);
            return RedirectToAction("Index");
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

        private CartItem CreateCartItem(Product product, int quantity)
        {
            return new CartItem
            {
                ProductId = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = quantity,
                Price = GetPrice(product, quantity)
            };
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
