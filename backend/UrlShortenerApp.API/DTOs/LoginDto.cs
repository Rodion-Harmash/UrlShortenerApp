using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApp.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
