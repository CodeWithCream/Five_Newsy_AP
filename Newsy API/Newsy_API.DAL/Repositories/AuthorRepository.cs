using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsy_API.DAL.Exceptions;
using Newsy_API.Model;
using Newtonsoft.Json;

namespace Newsy_API.DAL.Repositories
{
    public class AuthorRepository : RepositoryBase, IAuthorRepository
    {
        private readonly NewsyDbContext _context;
        private readonly ILogger<AuthorRepository> _logger;

        public AuthorRepository(NewsyDbContext context, ILogger<AuthorRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            _logger.LogInformation($"Querying all authors from database.");

            var authors = await _context.Authors
                .OrderBy(author => author.FirstName)
                .ThenBy(author => author.LastName)
                .ToListAsync();

            _logger.LogInformation($"{authors.Count} authors returned from database.");

            return authors;
        }

        public async Task<Author> GetByIdAsync(long id)
        {
            return await GetByIdAsync(id, false);
        }

        public async Task<Author> GetByIdAsync(long id, bool loadArticles)
        {
            _logger.LogInformation($"Querying author with id={id} from database.");

            IQueryable<Author> authorsSet = _context.Authors;
            if (loadArticles)
            {
                authorsSet = authorsSet.Include(author => author.Articles);
            }
            var author = await authorsSet.SingleOrDefaultAsync(author => author.Id == id);

            _logger.LogInformation($"Author {(author != null ? "found" : "not found")}.");

            if (author == null)
            {
                throw new NotFoundException();
            }

            return author;
        }

        public async Task InsertAsync(Author entity)
        {
            _logger.LogInformation($"Adding new auithor: {JsonConvert.SerializeObject(entity)}.");

            _context.Authors.Add(entity);
            await SaveAsync();

            _logger.LogInformation("Article added.");
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation($"Delete author whith id={id}.");

            var authorToDelete = await GetByIdAsync(id);
            _context.Authors.Remove(authorToDelete);
            await SaveAsync();

            _logger.LogInformation("Author deleted.");
        }
    }
}
