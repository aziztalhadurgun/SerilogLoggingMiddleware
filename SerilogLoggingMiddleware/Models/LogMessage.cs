namespace SerilogLoggingMiddleware.Models;

public class LogMessage
{
    public string Message { get; set; }
    public string Level { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Properties { get; set; } = new();
    public Exception Exception { get; set; }
    public string TraceId { get; set; }
    public string CorrelationId { get; set; }
    public string RequestPath { get; set; }
    public string HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public double? ElapsedMilliseconds { get; set; }
}