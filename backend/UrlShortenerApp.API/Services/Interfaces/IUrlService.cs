using System.Security.Claims;
using UrlShortenerApp.API.DTOs;

namespace UrlShortenerApp.API.Services.Interfaces
{
    public interface IUrlService
    {
        Task<ShortUrlDto> ShortenUrlAsync(string originalUrl, ClaimsPrincipal user);
        Task<List<ShortUrlDto>> GetAllAsync();
        Task<ShortUrlDto?> GetByIdAsync(int id, ClaimsPrincipal user);
        Task<bool> DeleteAsync(int id, ClaimsPrincipal user);
        Task<string?> ResolveShortCodeAsync(string shortCode);
    }
}
