using Bulky.DataAccess.Repository.IRepositories;
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
                var companyObj = _unitOfWork.GetRepository<Company>().Get(u => u.Id == id);
                if (companyObj == null)
                {
                    return NotFound();
                }
                return View(companyObj);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {

                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.GetRepository<Company>().Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.GetRepository<Company>().Update(CompanyObj);
                }

                await _unitOfWork.SaveChangesAsync();
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
            var companies = _unitOfWork.GetRepository<Company>().GetAll();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.GetRepository<Company>().Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.GetRepository<Company>().Remove(CompanyToBeDeleted);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
