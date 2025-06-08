using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Bulky.Utility.Attachments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles  = SD.Role_Admin)]
    public class ProductController(IUnitOfWork _unitOfWork, IAttachmentService attachmentService) : Controller
    {
        public IActionResult Index()
        {
          return View();
        }

        [HttpGet]

        public IActionResult Upsert(int? Id)
        {
            IEnumerable<SelectListItem> Categories = _unitOfWork.GetRepository<Category>().GetAll().Select(
              cat => new SelectListItem()
              {
                  Text = cat.Name,
                  Value = cat.Id.ToString()
              });

            var productVM = new ProductVM()
            {
                CategoryList = Categories,
                Product = new Product()
            };

            if (Id != null || Id > 0)
            {
                var product = _unitOfWork.GetRepository<Product>().Get(prod => prod.Id == Id);
                if (product == null)
                {
                    return NotFound();
                }
                productVM.Product = product;

            }

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<SelectListItem> Categories = _unitOfWork.GetRepository<Category>().GetAll().Select(
                cat => new SelectListItem()
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString()
                });
                productVM.CategoryList = Categories;
                return View(productVM);
            }
            if (productVM.Product.Id == 0)
            {
                if (file != null)
                    productVM.Product.ImageUrl = attachmentService.Upload(file, "Images\\Products")!;
                else
                    productVM.Product.ImageUrl = "PlaceHolder.png";

                _unitOfWork.GetRepository<Product>().Add(productVM.Product);

                TempData["success"] = "Product created successfully";
            }
            else
            {
                if (file != null)
                {
                    string path = Path.Combine("Images\\Products", productVM.Product.ImageUrl);
                    attachmentService.Delete(path);
                    productVM.Product.ImageUrl = attachmentService.Upload(file, "Images\\Products")!;
                }
                _unitOfWork.GetRepository<Product>().Update(productVM.Product);
                TempData["success"] = "Product updated successfully";
            }
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }
     
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.GetRepository<Product>().GetAll(p => p.Category);
            return Json(new { data = products });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? Id)
        {

            var product = _unitOfWork.GetRepository<Product>().Get(prod => prod.Id == Id);

            if (product == null)
                return Json(new { success = false, message = "Error while deleting" });

            string path = Path.Combine("Images\\Products", product.ImageUrl);
            attachmentService.Delete(path);

            _unitOfWork.GetRepository<Product>().Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });


        }

        

        #endregion

    }
}
