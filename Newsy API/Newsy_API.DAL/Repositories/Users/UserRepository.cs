using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsy_API.AuthenticationModel;
using Newsy_API.DAL.Exceptions;
using Newsy_API.Model;
using Newtonsoft.Json;

namespace Newsy_API.DAL.Repositories.Users
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private readonly NewsyDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(NewsyDbContext context, ILogger<UserRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApplicationUser> GetByEMailAsync(string email)
        {
            _logger.LogInformation($"Querying user with username={email} from database.");

            var applicationUser = await _context.Users
                .Include(applicationUser => applicationUser.User)
                .SingleOrDefaultAsync(x => x.UserName == email);

            _logger.LogInformation($"User {(applicationUser != null ? "found" : "not found")}.");

            if (applicationUser == null)
            {
                throw new NotFoundException();
            }
            return applicationUser;
        }

        public Task<User> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(User entity)
        {
            _logger.LogInformation($"Adding new user: {JsonConvert.SerializeObject(entity)}.");

            if (entity is Author)
            {
                _context.Authors.Add((Author)entity);
            }
            else
            {
                _context.Readers.Add(entity);
            }
            await SaveAsync();

            _logger.LogInformation("Article added.");
        }
    }
}
