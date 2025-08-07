namespace UrlShortenerApp.API.DTOs
{
    public class ShortUrlDto
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; } = null!;

        public string ShortCode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string CreatedByDisplayName { get; set; } = null!;

        public int ClickCount { get; set; }
    }
}
