using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newsy_API.DAL;
using Newsy_API.DTOs.Article;
using Newsy_API.Model;
using Newtonsoft.Json;

namespace Newsy_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
        {
            var articles = await _context.Articles
                .Include(article => article.Author)
                .OrderBy(article => article.Created)
                .ToListAsync();

            _logger.LogInformation($"{articles.Count} articles found.");

            return new OkObjectResult(_mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDto>>(articles));
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

            return _mapper.Map<ArticleDto>(article);
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
