namespace UrlShortenerApp.API.DTOs
{
    public class AboutDto
    {
        public string Content { get; set; } = null!;

        public DateTime LastEditedAt { get; set; }

        public string EditedByDisplayName { get; set; } = null!;
    }
}
