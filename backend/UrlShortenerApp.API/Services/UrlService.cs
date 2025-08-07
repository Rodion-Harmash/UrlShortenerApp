using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlShortenerApp.API.Data;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services.Interfaces;

namespace UrlShortenerApp.API.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext _context;

        public UrlService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShortUrlDto> ShortenUrlAsync(string originalUrl, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            var exists = await _context.ShortUrls
                .AnyAsync(x => x.OriginalUrl == originalUrl && x.CreatedById == userId);

            if (exists)
                throw new InvalidOperationException("This URL already exists.");

            var shortCode = GenerateShortCode();

            // Ensure uniqueness (very low chance of conflict, but check anyway)
            while (await _context.ShortUrls.AnyAsync(x => x.ShortCode == shortCode))
            {
                shortCode = GenerateShortCode();
            }

            var entity = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId!
            };

            _context.ShortUrls.Add(entity);
            await _context.SaveChangesAsync();

            var displayName = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync();

            return new ShortUrlDto
            {
                Id = entity.Id,
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CreatedAt = entity.CreatedAt,
                ClickCount = entity.ClickCount,
                CreatedByDisplayName = displayName ?? "Unknown"
            };
        }

        public async Task<List<ShortUrlDto>> GetAllAsync()
        {
            return await _context.ShortUrls
                .Include(x => x.CreatedBy)
                .Select(x => new ShortUrlDto
                {
                    Id = x.Id,
                    OriginalUrl = x.OriginalUrl,
                    ShortCode = x.ShortCode,
                    CreatedAt = x.CreatedAt,
                    ClickCount = x.ClickCount,
                    CreatedByDisplayName = x.CreatedBy.DisplayName
                })
                .ToListAsync();
        }

        public async Task<ShortUrlDto?> GetByIdAsync(int id, ClaimsPrincipal user)
        {
            var entity = await _context.ShortUrls
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            if (!IsAuthorized(user, entity))
                return null;

            return new ShortUrlDto
            {
                Id = entity.Id,
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CreatedAt = entity.CreatedAt,
                ClickCount = entity.ClickCount,
                CreatedByDisplayName = entity.CreatedBy.DisplayName
            };
        }

        public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
        {
            var entity = await _context.ShortUrls
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            if (!IsAuthorized(user, entity))
                return false;

            _context.ShortUrls.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> ResolveShortCodeAsync(string shortCode)
        {
            var entity = await _context.ShortUrls
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (entity == null)
                return null;

            entity.ClickCount++;
            await _context.SaveChangesAsync();

            return entity.OriginalUrl;
        }

        private static string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private static bool IsAuthorized(ClaimsPrincipal user, ShortUrl url)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = user.IsInRole("Admin");
            return isAdmin || url.CreatedById == userId;
        }
    }
}
