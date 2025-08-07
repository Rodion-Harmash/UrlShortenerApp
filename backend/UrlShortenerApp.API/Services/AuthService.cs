using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Models;
using UrlShortenerApp.API.Services.Interfaces;

namespace UrlShortenerApp.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var byName = await _userManager.FindByNameAsync(dto.UserName);
            var byEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (byName != null || byEmail != null)
            {
                var error = new IdentityError
                {
                    Code = "DuplicateUser",
                    Description = "User with this username or email already exists."
                };
                return IdentityResult.Failed(error);
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                RegisteredAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return result;

            await _userManager.AddToRoleAsync(user, "User");

            return IdentityResult.Success;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return null;

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return _tokenGenerator.GenerateToken(user, roles);
        }
    }
}
