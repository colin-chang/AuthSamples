using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ColinChang.IdentityServer.Api.Requirements
{
    public class ChineseAdministratorRequirement
        : IAuthorizationRequirement,
            IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.Identity is {IsAuthenticated: true} &&
                context.User.Claims.FirstOrDefault(c => c.Type == "nationality")?.Value == "China" &&
                context.User.IsInRole("Administrator"))
            {
                context.Succeed(this);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}