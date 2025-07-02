namespace SerilogLoggingMiddleware.Models;

// Document model for MongoDB
public class LogDocument
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string CorrelationId { get; set; }
    public string TraceId { get; set; }
    public string RequestPath { get; set; }
    public string HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public double? ElapsedMilliseconds { get; set; }
}