using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using EcommerceDotNetCore.AuthorizationRequirements;
using EcommerceDotNetCore.Models;

namespace EcommerceDotNetCore.AuthorizationHandlers
{
    public class EmailConfirmedRequirementHandler : AuthorizationHandler<EmailConfirmedRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailConfirmedRequirementHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailConfirmedRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;

            if (!context.User.Identity.IsAuthenticated)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Unauthorized Access");
                context.Fail(); 
                return;
            }

            var email = context.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                httpContext.Response.StatusCode = 409;
                await httpContext.Response.WriteAsync("Please confirm your email.");
                context.Fail();
                return;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && user.EmailConfirmed)
            {
                context.Succeed(requirement);
            }
        }
    }
}