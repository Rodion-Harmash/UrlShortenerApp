using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlShortenerApp.API.Data;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services.Interfaces;

namespace UrlShortenerApp.API.Services
{
    public class AboutService : IAboutService
    {
        private readonly ApplicationDbContext _context;

        public AboutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AboutDto?> GetAsync()
        {
            var about = await _context.AboutInfos
                .Include(x => x.EditedBy)
                .FirstOrDefaultAsync();

            if (about == null)
                return null;

            return new AboutDto
            {
                Content = about.Content,
                LastEditedAt = about.LastEditedAt,
                EditedByDisplayName = about.EditedBy.DisplayName
            };
        }

        public async Task<bool> UpdateAsync(string content, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = user.IsInRole("Admin");

            if (!isAdmin || string.IsNullOrWhiteSpace(userId))
                return false;

            var about = await _context.AboutInfos.FirstOrDefaultAsync();

            if (about == null)
            {
                about = new AboutInfo
                {
                    Content = content,
                    LastEditedAt = DateTime.UtcNow,
                    EditedById = userId
                };

                _context.AboutInfos.Add(about);
            }
            else
            {
                about.Content = content;
                about.LastEditedAt = DateTime.UtcNow;
                about.EditedById = userId;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
