using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MSN.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string? Url { get; set; }

        public string? UserId { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser? User { get; set; }
    }
}
