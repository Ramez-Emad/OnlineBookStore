using Bulky.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyWeb.ViewModels
{
    public class SummaryVM
    {
        public OrderHeader OrderHeader { get; set; } = default!;

        [ValidateNever]
        public IEnumerable<CartItem> CartItems { get; set; } = default!;
    }
}
