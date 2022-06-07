namespace Newsy_API.DTOs.Author
{
    public class AuthorBasicDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
