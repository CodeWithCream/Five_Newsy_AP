using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newsy_API.AuthenticationModel;
using Newsy_API.DAL.Exceptions;
using Newsy_API.DAL.Repositories.Users;
using Newsy_API.DTOs.User;
using Newsy_API.Model;
using Newsy_API.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Newsy_API.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticlesController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenSettings _tokenSettings;

        public AuthController(IUserRepository repository,
            IMapper mapper, ILogger<ArticlesController> logger,
            UserManager<ApplicationUser> userManager,
            IOptions<TokenSettings> tokenSettings)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _tokenSettings = tokenSettings.Value;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest();
            }

            ApplicationUser applicationUser;
            try
            {
                applicationUser = await _repository.GetByEMailAsync(loginDto.EMail);
            }
            catch (NotFoundException)
            {
                _logger.LogWarning($"User '{loginDto.EMail}' not found.");
                return NotFound();
            }

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(applicationUser);
            if (!emailConfirmed)
            {
                _logger.LogWarning($"Email '{loginDto.EMail}' not confirmed.");
                return BadRequest();
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(applicationUser, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"User '{loginDto.EMail}' tried to login with wrong password.");
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            var communicationToken = await LogInAsync(applicationUser);

            return Ok(new UserLoggedInDto() { Token = communicationToken, UserId = applicationUser.UserRefId });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterUser(RegisterUserDto registerUserDto)
        {
            using (var identitydbContextTransaction = _repository.BeginTransaction())
            {
                try
                {
                    var user = await CreateUserDataAsync(registerUserDto);

                    var identityResult = await CreateApplicationUserAsync(user, registerUserDto.EMail, registerUserDto.RoleKey, registerUserDto.Password);
                    if (!identityResult.Succeeded)
                    {
                        _logger.LogError("Error while registering application user.", identityResult.Errors);
                        identitydbContextTransaction.Rollback();
                        return BadRequest(/*TODO: send validation error, but not expose database data*/);
                    }

                    var applicationUser = await _repository.GetByEMailAsync(registerUserDto.EMail);

                    // TODO: the right way is to implement sending confirmation mail to user then expecting him to confirm it and set password
                    // implementation flow
                    // generate emailConfirmationToken
                    // generate callbackurl with this token
                    // send email with generated callback
                    // user clicks link and confirms email, then gets token for using the app

                    // but to make this app simple user will immidiately be registered

                    identitydbContextTransaction.Commit();

                    var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var confirmResult = await _userManager.ConfirmEmailAsync(applicationUser, emailToken);

                    var communicationToken = await LogInAsync(applicationUser);

                    var registrationData = new UserLoggedInDto() { Token = communicationToken, UserId = applicationUser.UserRefId };
                    return Ok(registrationData);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while creating user.");
                    identitydbContextTransaction.Rollback();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }

        private async Task<string> LogInAsync(ApplicationUser applicationUser)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret));
            var roles = await _userManager.GetRolesAsync(applicationUser);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, applicationUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString())
            });
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _logger.LogInformation($"user '{applicationUser.UserName}' has roles: '{string.Join(',', roles)}'");

            var tokenOptions = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                claims: claimsIdentity.Claims,
                expires: DateTime.Now.AddDays(_tokenSettings.AccessExpiration),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

        private async Task<User> CreateUserDataAsync(RegisterUserDto registerUserDto)
        {
            ApplicationUserRoles role = (ApplicationUserRoles)Enum.Parse(typeof(ApplicationUserRoles), registerUserDto.RoleKey, true);
            User user = role switch
            {
                ApplicationUserRoles.Author => _mapper.Map<RegisterUserDto, Author>(registerUserDto),
                ApplicationUserRoles.Reader => _mapper.Map<RegisterUserDto, User>(registerUserDto),
                _ => throw new ArgumentException($"User role {registerUserDto.RoleKey} not valid"),
            };

            await _repository.InsertAsync(user);

            return user;
        }

        private async Task<IdentityResult> CreateApplicationUserAsync(User user, string email, string roleKey, string password)
        {
            ApplicationUser applicationUser = new()
            {
                UserName = email,
                Email = email,
                User = user
            };

            var identityResult = await _userManager.CreateAsync(applicationUser, password);

            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            identityResult = await _userManager.AddToRoleAsync(applicationUser, roleKey.ToLower());

            return identityResult;
        }

    }
}
