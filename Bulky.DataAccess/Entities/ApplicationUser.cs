using Microsoft.AspNetCore.Identity;

namespace Bulky.DataAccess.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company? Company { get; set; } = default!;

        [NotMapped]
        public string Role { get; set; } = default!;
    }
}
