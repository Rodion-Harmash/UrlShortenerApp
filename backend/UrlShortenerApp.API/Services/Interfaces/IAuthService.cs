using UrlShortenerApp.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace UrlShortenerApp.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
