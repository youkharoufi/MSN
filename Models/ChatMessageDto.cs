using System.Text.Json.Serialization;
using Microsoft.AspNet.Identity;

namespace MSN.Models
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }

        public string SenderUsername { get; set; }
        [JsonIgnore]
        public ApplicationUser Sender { get; set; }

        public string TargetId { get; set; }

        public string TargetUsername { get; set; }
        [JsonIgnore]
        public ApplicationUser Target { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }
    }
}


