using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MSN.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }

        public string SenderUsername { get; set; }

        [JsonIgnore]
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }

        public string TargetId { get; set; }

        public string TargetUsername { get; set; }

        [JsonIgnore]
        [ForeignKey("TargetId")]
        public ApplicationUser Target { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    }
}
