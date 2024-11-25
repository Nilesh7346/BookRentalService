using Data;
using Models;

namespace Services.Helpers
{
    public class ActivityLogger
    {
        private readonly AppDbContext _context;

        public ActivityLogger(AppDbContext context)
        {
            _context = context;
        }

        // Log an activity
        public async Task LogActivityAsync(string logType, string message, string endpoint = null, int? userId = null, int? durationMs = null)
        {
            var log = new ActivityLog
            {
                LogType = logType,
                Message = message,
                Endpoint = endpoint,
                UserId = userId,
                DurationMs = durationMs,
                LogTime = DateTime.UtcNow
            };

            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }

}
