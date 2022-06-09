using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newsy_API.DAL.Exceptions;

namespace Newsy_API.DAL.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly NewsyDbContext _context;
        private readonly ILogger<RepositoryBase> _logger;

        public RepositoryBase(NewsyDbContext context, ILogger<RepositoryBase> logger)
        {
            _context = context;
            _logger = logger;
        }
        protected async Task SaveAsync()
        {
            _logger.LogInformation("Saving changes to database.");

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Changes saved.");
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(e, "There are conflicts while saving changes to database.");
                throw new ConflictException();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving changes.");
                throw new Exception("Error while saving changes.");
            }
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}
