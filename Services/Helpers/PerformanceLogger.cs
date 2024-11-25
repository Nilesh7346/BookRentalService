using Data;
using Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;


namespace Services.Helpers
{
    public class PerformanceLogger
    {
        private readonly RequestDelegate _next;

        public PerformanceLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context); // Process the request
            }
            finally
            {
                stopwatch.Stop();

                // Log performance metrics
                var log = new ActivityLog
                {
                    LogType = "Performance",
                    Message = $"Endpoint {context.Request.Path} executed.",
                    Endpoint = context.Request.Path,
                    DurationMs = (int)stopwatch.ElapsedMilliseconds,
                    LogTime = DateTime.UtcNow
                };

                await dbContext.ActivityLogs.AddAsync(log);
                await dbContext.SaveChangesAsync();
            }
        }
    }

}
