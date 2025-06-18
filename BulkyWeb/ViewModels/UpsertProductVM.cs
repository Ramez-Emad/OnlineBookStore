using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.ViewModels

{
    public class UpsertProductVM
    {
        public UpsertProductDto ProductDTO { get; set; } = new UpsertProductDto();
        public IEnumerable<SelectListItem> CategoryList { get; set; } = [];
    }
}
