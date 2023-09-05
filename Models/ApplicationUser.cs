using Microsoft.AspNetCore.Identity;

namespace MSN.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Photo? ProfilePic { get; set; }



    }
}
