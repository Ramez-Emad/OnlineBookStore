using Bulky.BL.Models.Categories;
using Bulky.BL.Services._ServicesManager;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController(IServicesManager _servicesManager) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var categories = await _servicesManager.CategoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var isCreated = await _servicesManager.CategoryService.CreateCategoryAsync(category);

            if(isCreated > 0)
                TempData["success"] = "Category is created successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? Id)
        {

            var category = await _servicesManager.CategoryService.GetCategoryByIdAsync(Id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var isUpdated = await _servicesManager.CategoryService.UpdateCategoryAsync(category);

            if(isUpdated > 0)
                TempData["success"] = "Category is updated successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var category = await _servicesManager.CategoryService.GetCategoryByIdAsync(Id);

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? Id)
        {
            var isDeleted = await _servicesManager.CategoryService.DeleteCategoryAsync(Id);

            if (isDeleted)
            {
                TempData["success"] = "Category is deleted successfully";
            }
            return RedirectToAction("Index");
        }

    }
}
