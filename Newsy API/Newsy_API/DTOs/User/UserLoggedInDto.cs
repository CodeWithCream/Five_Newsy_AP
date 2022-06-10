namespace Newsy_API.DTOs.User
{
    public class UserLoggedInDto
    {
        public string Token { get; set; } = null!;
        public long UserId { get; set; }
        public string UserType { get; set; } = null!;
    }
}
