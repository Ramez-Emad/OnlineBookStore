using Bulky.BL.Services._ServicesManager;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository._Generic;
using Bulky.Utility;
using BulkyWeb.Data;
using BulkyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController(IServicesManager _servicesManager, RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager ,  SignInManager<ApplicationUser> _signInManager) : Controller
    {

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> RoleManagment(string userId)
        {

            var user = await _servicesManager.UserService.GetUserByIdAsync(userId);

            if (user == null) {
                return NotFound();
            }
            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = user,

                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = (await _servicesManager.CompanyService.GetAllCompaniesAsync()).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;
            return View(RoleVM);
        }


        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var user = await _servicesManager.UserService.GetUserByIdAsync(roleManagmentVM.ApplicationUser.Id);

            if (user == null)
            {
                return NotFound();
            }
            string oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;
            string newrole = roleManagmentVM.ApplicationUser.Role;
            int? companyId = roleManagmentVM.ApplicationUser.CompanyId;

            if (oldRole == newrole)
            {
                if (newrole == SD.Role_Company)
                    user.CompanyId = companyId;
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, oldRole);
                user.Role = newrole;
                await _userManager.AddToRoleAsync(user, newrole);

                if (newrole == SD.Role_Company)
                {
                   user.CompanyId = companyId;
                }
                else
                {
                   user.CompanyId = null;
                }
            }

            await _servicesManager.UserService.UpdateUser(user);

            if (user.Id == _userManager.GetUserId(User))
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var objUserList = await _servicesManager.UserService.GetAllUsersAsync();

            foreach (var user in objUserList)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            var result = objUserList.Select(user => new
            {
                id = user.Id,
                CompanyName = user.Company?.Name ?? "",
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LockoutEnd = user.LockoutEnd,
                Role = user.Role
            });

            return Json(new { data = result });
        }


        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            await _servicesManager.UserService.ToggleLockUser(id);

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
