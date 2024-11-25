
namespace Models
{
    public class ActivityLog
    {
        public int Id { get; set; } // Primary key
        public string LogType { get; set; } = string.Empty; // Type of log: Info, Error, Performance
        public string Message { get; set; } = string.Empty; // Details of the log
        public DateTime LogTime { get; set; } = DateTime.UtcNow; // Timestamp of the log
        public string? Endpoint { get; set; } // API endpoint related to the log
        public int? UserId { get; set; } // User associated with the log (nullable)
        public int? DurationMs { get; set; } // Execution time in milliseconds (nullable)
    }

}
