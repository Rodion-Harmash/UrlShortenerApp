using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerApp.API.Models
{
    public class AboutInfo
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public DateTime LastEditedAt { get; set; }

        [Required]
        [ForeignKey("EditedBy")]
        public string EditedById { get; set; } = null!;
        public ApplicationUser EditedBy { get; set; } = null!;
    }
}
