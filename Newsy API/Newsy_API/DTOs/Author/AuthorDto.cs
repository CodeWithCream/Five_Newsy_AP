namespace Newsy_API.DTOs.Author
{
    public class AuthorDto : AuthorBasicDto
    {
        public string? Biography { get; set; }
        public string? Tagline { get; set; }
    }
}
