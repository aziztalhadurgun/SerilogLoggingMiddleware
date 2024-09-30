namespace SerilogLoggingMiddleware.Logging;

public class WriteToOption
{
    public string? Name { get; set; }
    public Dictionary<string, string>? Args { get; set; }
}