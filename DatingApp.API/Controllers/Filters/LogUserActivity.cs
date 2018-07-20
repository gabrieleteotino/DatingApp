using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Controllers.Filters
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();

            int userId = int.Parse(actionExecutedContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = actionExecutedContext.HttpContext.RequestServices.GetService<IDatingRepository>();

            var currentUser = await repo.GetUser(userId);

            currentUser.LastActive = DateTime.UtcNow;

            await repo.SaveAll();
        }
    }
}