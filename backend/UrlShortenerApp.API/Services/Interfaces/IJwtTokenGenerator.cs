using UrlShortenerApp.API.Models;

namespace UrlShortenerApp.API.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
