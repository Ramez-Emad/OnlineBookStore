using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class SummaryVM
    {
        public OrderHeader OrderHeader { get; set; } = default!;

        [ValidateNever]
        public IEnumerable<CartItem> CartItems { get; set; } = default!;
    }
}
