using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy_API.DAL;
using Newsy_API.Model;
using Microsoft.EntityFrameworkCore;
using Newsy_API.DTOs.Author;
using Newsy_API.DTOs.Article;

namespace Newsy_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly NewsyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(NewsyDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _context.Authors.ToListAsync();

            _logger.LogInformation($"{authors.Count} authors found.");

            return new OkObjectResult(_mapper.Map<IEnumerable<Author>, IEnumerable<AuthorBasicDto>>(authors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthor(long id)
        {
            var author = await _context.Authors
                .SingleOrDefaultAsync(author => author.Id == id);

            if (author == null)
            {
                _logger.LogWarning($"Author with id '{id}' does not exist.");
                return NotFound();
            }

            return new OkObjectResult(_mapper.Map<AuthorDto>(author));
        }

        [HttpGet("{id}/articles")]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles(long id)
        {
            var author = await _context.Authors
                .Include(author => author.Articles)
                .SingleOrDefaultAsync(author => author.Id == id);

            if (author == null)
            {
                _logger.LogWarning($"Author with id '{id}' does not exist.");
                return NotFound();
            }

            _logger.LogInformation($"Author has {author.Articles.Count} articles.");

            return new OkObjectResult(_mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDto>>(author.Articles));
        }
    }
}
