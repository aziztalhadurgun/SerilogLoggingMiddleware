namespace SerilogLoggingMiddleware.Models;

public class LogHandlerOptions
{
    public bool EnableConsole { get; set; } = true;
    public bool EnableFile { get; set; } = false;
    public bool EnableDatabase { get; set; } = false;
    public bool EnableSeq { get; set; } = false;

    public string FilePath { get; set; } = "logs/app.log";
    public string DatabaseConnectionString { get; set; }
    public string SeqServerUrl { get; set; }
    public string SeqApiKey { get; set; }
}