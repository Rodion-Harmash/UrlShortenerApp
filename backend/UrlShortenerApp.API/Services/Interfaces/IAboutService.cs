using System.Security.Claims;
using UrlShortenerApp.API.DTOs;

namespace UrlShortenerApp.API.Services.Interfaces
{
    public interface IAboutService
    {
        Task<AboutDto?> GetAsync();
        Task<bool> UpdateAsync(string content, ClaimsPrincipal user);
    }
}
