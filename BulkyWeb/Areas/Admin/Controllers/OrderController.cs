using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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


        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var ordersHeaders = _unitOfWork.GetRepository<OrderHeader>().GetAll(o => o.ApplicationUser);
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
