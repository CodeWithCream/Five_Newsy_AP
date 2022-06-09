using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newsy_API.AuthenticationModel;
using Newsy_API.DAL.Repositories.Articles;

namespace Newsy_API.Helpers
{
    /// <summary>
    /// Requirement that operation can be done only of articles created by author who is executing them
    /// </summary>
    public class MyArticlesRequirement : IAuthorizationRequirement
    {
    }

    public class MyArticlesRequirementHandler : AuthorizationHandler<MyArticlesRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IArticleRepository _repository;

        public MyArticlesRequirementHandler(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IArticleRepository repository)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager;
            _repository = repository;
        }

        private HttpContext HttpContext
        {
            get
            {
                return _httpContextAccessor.HttpContext;
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MyArticlesRequirement requirement)
        {
            var articleId = Convert.ToInt64(HttpContext.Request.RouteValues["id"]);

            var applicationUser = await _userManager.GetUserAsync(HttpContext.User);

            var article = await _repository.GetByIdAsync(articleId);

            if (applicationUser.UserRefId != article.AuthorId)
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
