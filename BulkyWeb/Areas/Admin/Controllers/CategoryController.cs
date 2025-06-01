using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using BulkyWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(SD.Role_Admin)]
    public class CategoryController(IUnitOfWork unitOfWork) : Controller
    {
      
        public IActionResult Index()
        {
            var categories = unitOfWork.CategoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            unitOfWork.CategoryRepository.Add(category);
            unitOfWork.Save();
            TempData["success"] = "Category is created successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();
            var category = unitOfWork.CategoryRepository.Get(cat => cat.Id == Id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            unitOfWork.CategoryRepository.Update(category);
            unitOfWork.Save();
            TempData["success"] = "Category is updated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();

            var category = unitOfWork.CategoryRepository.Get(cat => cat.Id == Id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost , ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            var category = unitOfWork.CategoryRepository.Get(cat => cat.Id == Id);

            if (category == null)
                return NotFound();

            unitOfWork.CategoryRepository.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Category is deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
