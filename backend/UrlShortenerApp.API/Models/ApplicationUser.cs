using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApp.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public override string Email { get; set; } = null!;

        [Required]
        public DateTime RegisteredAt { get; set; }

        public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();

        public ICollection<AboutInfo> EditedAbouts { get; set; } = new List<AboutInfo>();
    }
}
