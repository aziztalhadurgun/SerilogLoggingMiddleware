using Serilog;

namespace SerilogLoggingMiddleware.LogHandlers;

public class ConsoleLogHandler : ILogHandler
{
    public LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args)
    {
        return loggerConfig.WriteTo.Console();
    }
}