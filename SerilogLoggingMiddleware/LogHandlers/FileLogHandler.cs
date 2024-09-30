using Serilog;

namespace SerilogLoggingMiddleware.LogHandlers;

public class FileLogHandler : ILogHandler
{
    public LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args)
    {
        if (args.TryGetValue("path", out var path))
        {
            return loggerConfig.WriteTo.File(path);
        }
        throw new ArgumentException("File path must be provided for File logging.");
    }
}