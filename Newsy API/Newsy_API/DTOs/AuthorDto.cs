namespace Newsy_API.DTOs
{
    public class AuthorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Biography { get; set; }
        public string? Tagline { get; set; }

        public AuthorDto(string firstName, string lastName, string? biography, string? tagline)
        {
            FirstName = firstName;
            LastName = lastName;
            Biography = biography;
            Tagline = tagline;
        }
    }
}
