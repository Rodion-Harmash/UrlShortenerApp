using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlShortenerApp.API.Data;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services;
using UrlShortenerApp.API.Services.Interfaces;
using Xunit;

namespace UrlShortenerApp.Tests
{
    public class AboutServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IAboutService _service;
        private readonly ClaimsPrincipal _adminUser;
        private readonly ClaimsPrincipal _normalUser;

        public AboutServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new AboutService(_context);

            _adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-id"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _normalUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _service.GetAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnContent_WhenExists()
        {
            _context.Users.Add(new ApplicationUser
            {
                Id = "admin-id",
                DisplayName = "Admin",
                Email = "admin@example.com",
                UserName = "admin"
            });

            _context.AboutInfos.Add(new AboutInfo
            {
                Content = "Initial about text",
                LastEditedAt = DateTime.UtcNow,
                EditedById = "admin-id"
            });

            await _context.SaveChangesAsync();

            var result = await _service.GetAsync();

            Assert.NotNull(result);
            Assert.Equal("Initial about text", result.Content);
            Assert.Equal("Admin", result.EditedByDisplayName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCreateNewRecord_WhenNoneExists()
        {
            var result = await _service.UpdateAsync("About test content", _adminUser);

            var about = await _context.AboutInfos.FirstOrDefaultAsync();

            Assert.True(result);
            Assert.NotNull(about);
            Assert.Equal("About test content", about?.Content);
            Assert.Equal("admin-id", about?.EditedById);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingRecord()
        {
            _context.AboutInfos.Add(new AboutInfo
            {
                Content = "Old content",
                LastEditedAt = DateTime.UtcNow.AddDays(-1),
                EditedById = "someone"
            });

            await _context.SaveChangesAsync();

            var result = await _service.UpdateAsync("Updated content", _adminUser);

            var about = await _context.AboutInfos.FirstOrDefaultAsync();

            Assert.True(result);
            Assert.NotNull(about);
            Assert.Equal("Updated content", about?.Content);
            Assert.Equal("admin-id", about?.EditedById);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReject_WhenUserIsNotAdmin()
        {
            var result = await _service.UpdateAsync("Should not be saved", _normalUser);

            Assert.False(result);
            Assert.Empty(_context.AboutInfos);
        }
    }
}
