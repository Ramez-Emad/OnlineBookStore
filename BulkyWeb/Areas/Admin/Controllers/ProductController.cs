using Bulky.BL.Models.Products;
using Bulky.BL.Services._ServicesManager;
using Bulky.DataAccess.Exceptions;
using Bulky.Utility;
using BulkyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController(IServicesManager _servicesManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Upsert(int? Id)
        {
            var categories = await _servicesManager.CategoryService.GetAllCategoriesAsync();

            var CategoriesList = categories.Select( cat => new SelectListItem()
              {
                  Text = cat.Name,
                  Value = cat.Id.ToString()
              });

            var upsertProductVM = new UpsertProductVM()
            {
                CategoryList = CategoriesList
            };

            if (Id != null || Id > 0)
            {
                var product = await _servicesManager.ProductService.GetProductByIdAsync(Id);
                upsertProductVM.ProductDTO = _servicesManager.Mapper.Map<UpsertProductDto>(product);
            }
            
            return View(upsertProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(UpsertProductVM productVM, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _servicesManager.CategoryService.GetAllCategoriesAsync();

                var CategoriesList = categories.Select(cat => new SelectListItem()
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString()
                });

                productVM.CategoryList = CategoriesList;
                return View(productVM);
            }

            if (productVM.ProductDTO.Id == 0)
            {
                if (file != null)
                    productVM.ProductDTO.ImageUrl = _servicesManager.AttachmentService.UploadProductImage(file)!;
                else
                    productVM.ProductDTO.ImageUrl = "PlaceHolder.png";

                await _servicesManager.ProductService.CreateProductAsync(productVM.ProductDTO);

                TempData["success"] = "Product created successfully";
            }
            else
            {
                if (file != null)
                {
                    productVM.ProductDTO.ImageUrl = _servicesManager.AttachmentService.UpdateProductImage(productVM.ProductDTO.ImageUrl, file)!;
                }
                await _servicesManager.ProductService.UpdateProductAsync(productVM.ProductDTO);
                TempData["success"] = "Product updated successfully";
            }
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _servicesManager.ProductService.GetAllProductsAsync();
            return Json(new { data = products });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? Id)
        {

            var product = await _servicesManager.ProductService.GetProductByIdAsync(Id);

            if (product == null)
                return Json(new { success = false, message = "Error while deleting" });

            _servicesManager.AttachmentService.DeleteProductImage(product.ImageUrl);

            await _servicesManager.ProductService.DeleteProductAsync(Id);

            return Json(new { success = true, message = "Delete Successful" });
        }

       
        #endregion

        }

    
}