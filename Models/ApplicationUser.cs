using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MSN.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }

        public string Token { get; set; }

        public string PhotoUrl { get; set; }

        public List<ApplicationUser>? Friends { get; set; }

        [JsonIgnore]
        public List<ChatMessage> MessagesSent { get; set; }

        [JsonIgnore]
        public List<ChatMessage> MessagesRecieved { get; set; }

    }
}
