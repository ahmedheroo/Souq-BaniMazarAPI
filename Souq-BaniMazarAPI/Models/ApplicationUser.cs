using Microsoft.AspNetCore.Identity;

namespace Souq_BaniMazarAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; }
        public string NationalIdUrl { get; set; }
        public bool IsApproved { get; set; }

    }
}
