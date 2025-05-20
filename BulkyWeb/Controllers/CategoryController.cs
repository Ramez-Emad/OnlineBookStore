using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var categories = _db.Categories.ToList();
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
            _db.Categories.Add(category);
            _db.SaveChanges();
            TempData["success"] = "Category is created successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();
            var category = _db.Categories.Find(Id);
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
            _db.Categories.Update(category);
            _db.SaveChanges();
            TempData["success"] = "Category is updated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id <= 0)
                return NotFound();

            var category = _db.Categories.Find(Id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost , ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            var category = _db.Categories.Find(Id);

            if (category == null)
                return NotFound();

            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["success"] = "Category is deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
