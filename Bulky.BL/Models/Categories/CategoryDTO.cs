using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Models.Categories
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Name { get; set; } = default!;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
