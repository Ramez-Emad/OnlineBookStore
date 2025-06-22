using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.ViewModels
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; } = default!;
        public IEnumerable<SelectListItem> RoleList { get; set; } = default!;
        public IEnumerable<SelectListItem> CompanyList { get; set; } = default!;
    }
}
