using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsy_API.DAL.Exceptions;
using Newsy_API.Model;
using Newtonsoft.Json;

namespace Newsy_API.DAL.Repositories.Articles
{
    public class ArticleRepository : RepositoryBase, IArticleRepository
    {
        private readonly NewsyDbContext _context;
        private readonly ILogger<ArticleRepository> _logger;

        public ArticleRepository(NewsyDbContext context, ILogger<ArticleRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Article>> GetAllAsync(int pageSize, int pageNumber)
        {
            _logger.LogInformation($"Querying {pageSize} articles on page {pageNumber} from database.");

            var articles = await _context.Articles
                .Include(article => article.Author)
                .OrderBy(article => article.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation($"{articles.Count} articles returned from database.");

            return articles;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Articles.CountAsync();
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            _logger.LogInformation($"Querying all articles from database.");

            var articles = await _context.Articles
                .Include(article => article.Author)
                .ToListAsync();

            _logger.LogInformation($"{articles.Count} articles returned from database.");

            return articles;
        }

        public async Task<Article> GetByIdAsync(long id)
        {
            _logger.LogInformation($"Querying article with id={id} from database.");

            var article = await _context.Articles
                 .Include(article => article.Author)
                 .SingleOrDefaultAsync(article => article.Id == id);

            _logger.LogInformation($"Article {(article != null ? "found" : "not found")}.");

            if (article == null)
            {
                throw new NotFoundException();
            }

            return article;
        }

        public async Task InsertAsync(Article entity)
        {
            _logger.LogInformation($"Adding new  article: {JsonConvert.SerializeObject(entity)}.");

            _context.Articles.Add(entity);
            await SaveAsync();

            _logger.LogInformation("Article added.");
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation($"Delete article whith id={id}.");

            var articleToDelete = await GetByIdAsync(id);
            _context.Articles.Remove(articleToDelete);
            await SaveAsync();

            _logger.LogInformation("Article deleted.");
        }

        public async Task UpdateAsync(long id, string title, string text)
        {
            _logger.LogInformation($"Update article whith id={id}.");

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                _logger.LogError($"Article with id '{id}' does not exist.");
                throw new NotFoundException();
            }

            article.ChangeContent(title, text);
            _context.Entry(article).State = EntityState.Modified;
            await SaveAsync();

            _logger.LogInformation("Article updated.");
        }
    }
}
