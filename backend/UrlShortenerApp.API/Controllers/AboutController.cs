using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Services.Interfaces;

namespace UrlShortenerApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var result = await _aboutService.GetAsync();
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] string content)
        {
            var success = await _aboutService.UpdateAsync(content, User);
            if (!success)
                return Forbid();

            return NoContent();
        }
    }
}
