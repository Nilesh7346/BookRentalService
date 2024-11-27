
using Data.Interfaces;
using Models;

namespace Services.Helpers
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityLogger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Log an activity
        public async Task LogActivityAsync(string logType, string message, string endpoint = null, int? userId = null, int? durationMs = null)
        {
            try
            {
                await _unitOfWork.ActivityLogger.LogActivityAsync(logType, message, endpoint, userId, durationMs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddActivityAsync(ActivityLog log)
        {
            try
            {
                await _unitOfWork.ActivityLogger.AddActivityAsync(log);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
