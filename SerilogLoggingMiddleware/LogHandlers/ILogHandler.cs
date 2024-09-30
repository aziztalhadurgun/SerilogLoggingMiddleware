using Serilog;

namespace SerilogLoggingMiddleware.LogHandlers;

public interface ILogHandler
{
    LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args);
}