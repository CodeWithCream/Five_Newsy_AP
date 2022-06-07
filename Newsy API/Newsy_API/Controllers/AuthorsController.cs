using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy_API.DAL;
using Newsy_API.Model;
using Microsoft.EntityFrameworkCore;
using Newsy_API.DTOs.Author;

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

            return new OkObjectResult(_mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(authors));
        }
    }
}
