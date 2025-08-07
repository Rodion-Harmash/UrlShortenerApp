using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlShortenerApp.API.Data;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services;
using UrlShortenerApp.API.Services.Interfaces;
using Xunit;

namespace UrlShortenerApp.Tests
{
    public class UrlServiceTests
    {
        private readonly IUrlService _service;
        private readonly ApplicationDbContext _context;
        private readonly ClaimsPrincipal _user;

        public UrlServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var user = new ApplicationUser
            {
                Id = "user-123",
                DisplayName = "Test User",
                Email = "test@example.com",
                UserName = "testuser",
                RegisteredAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            _service = new UrlService(_context);

            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
        }

        [Fact]
        public async Task ShortenUrlAsync_ShouldCreateNewShortUrl()
        {
            var result = await _service.ShortenUrlAsync("https://example.com", _user);

            Assert.NotNull(result);
            Assert.Equal("https://example.com", result.OriginalUrl);
            Assert.NotEmpty(result.ShortCode);
        }

        [Fact]
        public async Task ShortenUrlAsync_ShouldThrowOnDuplicate()
        {
            await _service.ShortenUrlAsync("https://example.com", _user);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.ShortenUrlAsync("https://example.com", _user);
            });
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUrls()
        {
            await _service.ShortenUrlAsync("https://google.com", _user);
            await _service.ShortenUrlAsync("https://microsoft.com", _user);

            var list = await _service.GetAllAsync();

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItemIfAuthorized()
        {
            var created = await _service.ShortenUrlAsync("https://test.com", _user);

            var result = await _service.GetByIdAsync(created.Id, _user);

            Assert.NotNull(result);
            Assert.Equal("https://test.com", result.OriginalUrl);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteOwnUrl()
        {
            var created = await _service.ShortenUrlAsync("https://delete.com", _user);

            var success = await _service.DeleteAsync(created.Id, _user);
            Assert.True(success);

            var check = await _context.ShortUrls.FindAsync(created.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task ResolveShortCodeAsync_ShouldReturnOriginalUrlAndIncrementCount()
        {
            var created = await _service.ShortenUrlAsync("https://redirect.com", _user);

            var original = await _service.ResolveShortCodeAsync(created.ShortCode);
            Assert.Equal("https://redirect.com", original);

            var stored = await _context.ShortUrls.FindAsync(created.Id);
            Assert.Equal(1, stored?.ClickCount);
        }
    }
}
