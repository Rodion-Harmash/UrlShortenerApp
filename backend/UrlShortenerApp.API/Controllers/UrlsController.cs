using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortenerApp.API.DTOs;
using UrlShortenerApp.API.Services.Interfaces;

namespace UrlShortenerApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlsController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ShortUrlDto>>> GetAll()
        {
            var result = await _urlService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] string originalUrl)
        {
            try
            {
                var result = await _urlService.ShortenUrlAsync(originalUrl, User);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _urlService.GetByIdAsync(id, User);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _urlService.DeleteAsync(id, User);
            if (!success)
                return Forbid();

            return NoContent();
        }

        [HttpGet("/s/{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var url = await _urlService.ResolveShortCodeAsync(shortCode);
            if (url == null)
                return NotFound();

            return Redirect(url);
        }
    }
}
