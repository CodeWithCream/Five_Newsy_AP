using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newsy_API.DAL;
using Newsy_API.DTOs;
using Newsy_API.DTOs.Article;
using Newsy_API.Model;
using Newtonsoft.Json;

namespace Newsy_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly NewsyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(NewsyDbContext context, IMapper mapper, ILogger<ArticlesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve a list of article on given page 
        /// </summary>
        /// <param name="filter">Pagination parameters - page number and page size</param>
        /// <returns>Articles on requested page</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<IEnumerable<ArticleDto>>>> GetArticles([FromQuery] PaginationFilter filter)
        {
            var articlesCount = await _context.Articles.CountAsync();
            var totalPages = (int)Math.Ceiling(articlesCount * 1.0 / filter.PageSize);

            if (filter.PageNumber > totalPages)
            {
                _logger.LogError(
                    $"Requesting page {filter.PageNumber} with pagesize of {filter.PageSize}, but there are only {articlesCount} articles saved");
                return new BadRequestResult();
            }

            var articles = await _context.Articles
                .Include(article => article.Author)
                .OrderBy(article => article.Created)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            _logger.LogInformation($"{articlesCount} articles found. Sending {articles.Count} articles on page {filter.PageNumber}");

            var filteredArticles = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDto>>(articles);

            return new OkObjectResult(
                new PagedResult<IEnumerable<ArticleDto>>(filteredArticles,
                    filter.PageNumber,
                    articles.Count,
                    totalPages,
                    articlesCount));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticle(long id)
        {
            var article = await _context.Articles
                .Include(article => article.Author)
                .SingleOrDefaultAsync(article => article.Id == id);

            if (article == null)
            {
                _logger.LogWarning($"Article with id '{id}' does not exist.");
                return NotFound();
            }

            return new OkObjectResult(_mapper.Map<ArticleDto>(article));
        }

        [HttpPost]
        public async Task<ActionResult<CreateArticleDto>> PostReport(CreateArticleDto createArticleDto)
        {
            _logger.LogInformation($"Creating new article: {JsonConvert.SerializeObject(createArticleDto)}");

            var articleToCreate = _mapper.Map<Article>(createArticleDto);

            _context.Articles.Add(articleToCreate);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating article.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            var cretaedArticle = await _context.Articles
                .Include(article => article.Author)
                .SingleOrDefaultAsync(article => article.Id == articleToCreate.Id);
            var createdArticleDto = _mapper.Map<ArticleDto>(cretaedArticle);

            return CreatedAtAction(nameof(GetArticle), new { id = articleToCreate.Id }, createdArticleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditRArticle(long id, EditArticleDto editArticleDto)
        {
            if (id != editArticleDto.Id)
            {
                return BadRequest();
            }

            _logger.LogInformation($"Updating article: {JsonConvert.SerializeObject(editArticleDto)}");

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                _logger.LogWarning($"Article with id '{id}' does not exist,");
                return NotFound();
            }

            article.ChangeContent(editArticleDto.Title, editArticleDto.Text);
            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    _logger.LogError($"Article with id '{id}' does not exist,");
                    return NotFound();
                }
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating article.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var articleToDelete = await _context.Articles.FindAsync(id);
            if (articleToDelete == null)
            {
                _logger.LogError($"Article with id '{id}' does not exist,");
                return NotFound();
            }

            _context.Articles.Remove(articleToDelete);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    //already deleted
                    return new StatusCodeResult(StatusCodes.Status200OK);
                }
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting article.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        private bool ArticleExists(long id)
        {
            return _context.Articles.Any(article => article.Id == id);
        }
    }
}
