using Microsoft.AspNetCore.Identity;
using Newsy_API.Model;

namespace Newsy_API.AuthenticationModel
{
    public class ApplicationUser : IdentityUser
    {
        public long UserRefId { get; set; }
        public User User { get; set; }
        public bool IsActivated { get; set; } = true;

        public ApplicationUser(User user)
        {
            User = user;
        }
    }
}