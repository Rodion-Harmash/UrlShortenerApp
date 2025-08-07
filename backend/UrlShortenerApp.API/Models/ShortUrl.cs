using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UrlShortenerApp.API.Models
{
    [Index(nameof(ShortCode), IsUnique = true)]
    public class ShortUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Url]
        public string OriginalUrl { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ShortCode { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public string CreatedById { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;

        public int ClickCount { get; set; } = 0;
    }
}
