using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using Bulky.Utility;
using BulkyWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController(IUnitOfWork unitOfWork) : Controller
    {
      
        public IActionResult Index()
        {
            var categories = unitOfWork.GetRepository<Category>().GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            unitOfWork.GetRepository<Category>().Add(category);
            await unitOfWork.SaveChangesAsync();
            TempData["success"] = "Category is created successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();
            var category = unitOfWork.GetRepository<Category>().Get(cat => cat.Id == Id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            unitOfWork.GetRepository<Category>().Update(category);
            await unitOfWork.SaveChangesAsync();
            TempData["success"] = "Category is updated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();

            var category = unitOfWork.GetRepository<Category>().Get(cat => cat.Id == Id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost , ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? Id)
        {
            var category = unitOfWork.GetRepository<Category>().Get(cat => cat.Id == Id);

            if (category == null)
                return NotFound();

            unitOfWork.GetRepository<Category>().Remove(category);
            await unitOfWork.SaveChangesAsync();
            TempData["success"] = "Category is deleted successfully";
            return RedirectToAction("Index");
        }

       

    }
}
