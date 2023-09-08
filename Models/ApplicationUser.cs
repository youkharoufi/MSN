using Microsoft.AspNetCore.Identity;

namespace MSN.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }

        public string Token { get; set; }

        public string PhotoUrl { get; set; }

    }
}
