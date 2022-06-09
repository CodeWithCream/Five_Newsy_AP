using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public string EMail { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
