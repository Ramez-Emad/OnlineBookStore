using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Cart
    {
        public string Id { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];

        public double TotalCost { get; set; }
    }
}
