using System.ComponentModel.DataAnnotations.Schema;

namespace MSN.Models
{
    public class RegisterUser
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public IFormFile File { get; set; }

        public string? Link { get; set; }
    }
}
