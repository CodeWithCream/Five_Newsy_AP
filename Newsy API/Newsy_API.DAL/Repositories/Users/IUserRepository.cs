using Newsy_API.AuthenticationModel;
using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Finds application user by email
        /// </summary>
        /// <param name="email">User email"</param>
        /// <returns>Found user</returns>
        /// <exception cref="typeof(NotFoundException)">Application usewr not found or doesn't have referenced User data</exception>
        Task<ApplicationUser> GetByEMailAsync(string email);
    }
}
