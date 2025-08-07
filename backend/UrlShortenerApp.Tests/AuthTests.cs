using Microsoft.AspNetCore.Identity;
using Moq;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services;
using UrlShortenerApp.API.Services.Interfaces;
using Xunit;

namespace UrlShortenerApp.Tests
{
    public class AuthTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IJwtTokenGenerator> _tokenGeneratorMock;
        private readonly AuthService _authService;

        public AuthTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _tokenGeneratorMock = new Mock<IJwtTokenGenerator>();

            _authService = new AuthService(_userManagerMock.Object, _tokenGeneratorMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess()
        {
            var dto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                DisplayName = "Test User"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync((ApplicationUser?)null);

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser?)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterAsync(dto);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_OnDuplicate()
        {
            var dto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                DisplayName = "Test User"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync(new ApplicationUser { UserName = dto.UserName });

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await _authService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Code == "DuplicateUser");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken()
        {
            var dto = new LoginDto
            {
                UserName = "testuser",
                Password = "Test123!"
            };

            var user = new ApplicationUser
            {
                Id = "user-123",
                UserName = dto.UserName,
                DisplayName = "Test User",
                Email = "test@example.com"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            _tokenGeneratorMock.Setup(x => x.GenerateToken(user, It.IsAny<IList<string>>()))
                .Returns("mocked-jwt-token");

            var token = await _authService.LoginAsync(dto);

            Assert.NotNull(token);
            Assert.Equal("mocked-jwt-token", token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_OnInvalidCredentials()
        {
            var dto = new LoginDto
            {
                UserName = "wronguser",
                Password = "wrongpass"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync((ApplicationUser?)null);

            var token = await _authService.LoginAsync(dto);

            Assert.Null(token);
        }
    }
}
