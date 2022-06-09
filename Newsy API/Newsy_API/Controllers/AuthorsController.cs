using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy_API.DAL.Exceptions;
using Newsy_API.DAL.Repositories.Authors;
using Newsy_API.DTOs.Article;
using Newsy_API.DTOs.Author;
using Newsy_API.Model;

namespace Newsy_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorRepository repository, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _repository.GetAllAsync();

            _logger.LogInformation($"{authors.Count()} authors found.");

            return new OkObjectResult(_mapper.Map<IEnumerable<Author>, IEnumerable<AuthorBasicDto>>(authors));
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthor(long id)
        {
            try
            {
                var author = await _repository.GetByIdAsync(id);
                return new OkObjectResult(_mapper.Map<AuthorDto>(author));
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Author with id={id} does not exist.");
                return NotFound(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while trying to get author with id={id}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("{id}/articles")]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles(long id)
        {
            try
            {
                var author = await _repository.GetByIdAsync(id, true);
                return new OkObjectResult(_mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDto>>(author.Articles));
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Author with id={id} does not exist.");
                return NotFound(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while trying to get articles of author with id={id}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
