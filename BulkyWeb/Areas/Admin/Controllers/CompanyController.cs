using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController(IUnitOfWork _unitOfWork) : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                var companyObj = _unitOfWork.CompanyRepository.Get(u => u.Id == id);
                if (companyObj == null)
                {
                    return NotFound();
                }
                return View(companyObj);
            }

        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {

                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(CompanyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {

                return View(CompanyObj);
            }
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.CompanyRepository.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.CompanyRepository.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
