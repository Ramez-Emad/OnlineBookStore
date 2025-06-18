using Bulky.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Models.Products
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public string Author { get; set; } = default!;
        public double ListPrice { get; set; }
        public double Price100 { get; set; }
        public string Category { get; set; } = default!;
        public string ImageUrl { get; set; } = string.Empty;


    }
}
