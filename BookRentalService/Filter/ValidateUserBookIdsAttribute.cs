﻿
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ValidateUserBookIdsAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        private AppDbContext _context;

        public ValidateUserBookIdsAttribute()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (_context == null)
            {
                _context = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            }
            // Get parameters from the route or body
            var userIdKey = context.ActionArguments.Keys.FirstOrDefault(k => k.Equals("userId", StringComparison.OrdinalIgnoreCase));
            var bookIdKey = context.ActionArguments.Keys.FirstOrDefault(k => k.Equals("bookId", StringComparison.OrdinalIgnoreCase));
            int userId = 0;
            int bookId = 0;

            if (userIdKey != null && context.ActionArguments[userIdKey] != null)
            {
                if (int.TryParse(context.ActionArguments[userIdKey]?.ToString(), out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                else
                {
                    context.Result = new BadRequestObjectResult("Invalid user ID format.");
                    return;
                }
            }

            // Safely retrieve and parse bookId if it exists
            if (bookIdKey != null && context.ActionArguments[bookIdKey] != null)
            {
                if (int.TryParse(context.ActionArguments[bookIdKey]?.ToString(), out int parsedBookId))
                {
                    bookId = parsedBookId;
                }
                else
                {
                    context.Result = new BadRequestObjectResult("Invalid book ID format.");
                    return;
                }
            }

            // Validate if the UserId exists in the database
            if (userId != 0 && !await _context.Users.AnyAsync(u => u.Id == userId))
            {
                context.Result = new BadRequestObjectResult("Invalid user ID.");
                return; // Short-circuit the pipeline and don't call the action method
            }

            // Validate if the BookId exists in the database
            if (bookId != 0 && !await _context.Books.AnyAsync(b => b.Id == bookId))
            {
                context.Result = new BadRequestObjectResult("Invalid book ID.");
                return; // Short-circuit the pipeline and don't call the action method
            }
            // Proceed to the action if validation passes
            await next();
        }
    }

}