using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy_API.DAL.Exceptions;
using Newsy_API.DAL.Repositories;
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
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IArticleRepository repository, IMapper mapper, ILogger<ArticlesController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<IEnumerable<ArticleDto>>>> GetArticles([FromQuery] PaginationFilter filter)
        {
            var articlesCount = await _repository.CountAsync();
            var totalPages = (int)Math.Ceiling(articlesCount * 1.0 / filter.PageSize);

            if (filter.PageNumber > totalPages)
            {
                _logger.LogError(
                    $"Requesting page {filter.PageNumber} with pagesize of {filter.PageSize}, but there are not enough articles.");
                return new BadRequestResult();
            }

            var articles = await _repository.GetAllAsync(filter.PageSize, filter.PageNumber);

            _logger.LogInformation($"{articlesCount} articles found. Sending {articles.Count()} articles on page {filter.PageNumber}");

            var filteredArticles = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDto>>(articles);

            return new OkObjectResult(
                new PagedResult<IEnumerable<ArticleDto>>(filteredArticles,
                    filter.PageNumber,
                    articles.Count(),
                    totalPages,
                    articlesCount));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArticleDto>> GetArticle(long id)
        {
            try
            {
                var article = await _repository.GetByIdAsync(id);
                return new OkObjectResult(_mapper.Map<ArticleDto>(article));
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Article with id={id} does not exist.");
                return NotFound(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while trying to get article with id={id}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateArticleDto>> PostReport(CreateArticleDto createArticleDto)
        {
            _logger.LogInformation($"Creating new article: {JsonConvert.SerializeObject(createArticleDto)}.");

            var articleToCreate = _mapper.Map<Article>(createArticleDto);
            try
            {
                await _repository.InsertAsync(articleToCreate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            try
            {
                var createdArticle = await _repository.GetByIdAsync(articleToCreate.Id);
                var createdArticleDto = _mapper.Map<ArticleDto>(createdArticle);
                return CreatedAtAction(nameof(GetArticle), new { id = articleToCreate.Id }, createdArticleDto);
            }
            catch (NotFoundException) //someone deleted it
            {
                _logger.LogWarning("Someone deleted created article.");
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EditArticle(long id, EditArticleDto editArticleDto)
        {
            _logger.LogInformation($"Updating article: {JsonConvert.SerializeObject(editArticleDto)}");

            try
            {
                await _repository.UpdateAsync(id, editArticleDto.Title, editArticleDto.Text);
                _logger.LogInformation($"Article is updated.");

                return NoContent();
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Article with id={id} does not exist.");
                return NotFound(id);
            }
            catch (ConflictException)
            {
                _logger.LogError($"Update article conflict. Article is probably not updated.");
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogInformation($"Deleting article with id={id}.");

            try
            {
                await _repository.DeleteAsync(id);
                _logger.LogInformation($"Article is deleted.");

                return Ok();
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"Article with id={id} does not exist.");
                return NotFound(id);
            }
            catch (ConflictException)
            {
                //someone already deleted it
                _logger.LogWarning($"Delete article conflict. Article is already deleted.");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
        }
    }
}
