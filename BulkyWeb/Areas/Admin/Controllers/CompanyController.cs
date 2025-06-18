using Bulky.BL.Models.Companies;
using Bulky.BL.Services._ServicesManager;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController(IServicesManager _servicesManager) : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var companyDto = new CompanyDto();

            if (id != null || id > 0)
            {
                //update
                companyDto = await _servicesManager.CompanyService.GetCompanyByIdAsync(id);
            }

            return View(companyDto);


        }

        [HttpPost]
        public async Task<IActionResult> Upsert(CompanyDto CompanyObj)
        {
            if (!ModelState.IsValid)
                return View(CompanyObj);


            if (CompanyObj.Id == 0)
            {
                await _servicesManager.CompanyService.CreateCompanyAsync(CompanyObj);
                TempData["success"] = "Company created successfully";

            }
            else
            {
                await _servicesManager.CompanyService.UpdateCompanyAsync(CompanyObj);
                TempData["success"] = "Company updated successfully";

            }

            return RedirectToAction("Index");

        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _servicesManager.CompanyService.GetAllCompaniesAsync();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var CompanyToBeDeleted = await _servicesManager.CompanyService.GetCompanyByIdAsync(id.Value);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _servicesManager.CompanyService.DeleteCompanyAsync(id.Value);

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
