using Models;
using Data.Interfaces;

namespace Data.Repositories
{
    public class ActivityLoggerRepository : IActivityLoggerRepository
    {
        private readonly AppDbContext _context;

        public ActivityLoggerRepository(AppDbContext context)
        {
            _context = context;
        }
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
        public async Task AddActivityAsync(ActivityLog log)
        {
            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
