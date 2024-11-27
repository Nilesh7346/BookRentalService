using Models;

namespace Services.Helpers
{
    public interface IActivityLogger
    {
        public Task LogActivityAsync(string logType, string message, string endpoint = null, int? userId = null, int? durationMs = null);
        public Task AddActivityAsync(ActivityLog log);
    }
}
