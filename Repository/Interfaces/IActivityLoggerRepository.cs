using Models;


namespace Data.Interfaces
{
    public interface IActivityLoggerRepository 
    {
        public Task LogActivityAsync(string logType, string message, string endpoint = null, int? userId = null, int? durationMs = null);
        public Task AddActivityAsync(ActivityLog log);
    }
}
