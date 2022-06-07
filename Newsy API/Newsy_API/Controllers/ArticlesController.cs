using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newsy_API.DAL;
using Newsy_API.DTOs.Article;
using Newsy_API.Model;

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
    }
}
