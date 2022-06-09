using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newsy_API.AuthenticationModel;

namespace Newsy_API.Helpers
{
    /// <summary>
    /// Requirement that operation can be done only if data belongs to user executing it
    /// </summary>
    public class MyDataRequirement : IAuthorizationRequirement
    {
    }

    public class MyDataRequirementHandler : AuthorizationHandler<MyDataRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyDataRequirementHandler(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager;
        }

        private HttpContext HttpContext
        {
            get
            {
                return _httpContextAccessor.HttpContext;
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MyDataRequirement requirement)
        {
            var id = Convert.ToInt64(HttpContext.Request.RouteValues["id"]);

            var applicationUser = await _userManager.GetUserAsync(HttpContext.User);

            if (applicationUser.UserRefId != id)
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
