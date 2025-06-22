using Serilog;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public interface ILogHandler
{
    // LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args);
    void Configure(LoggerConfiguration loggerConfiguration);
    Task HandleLogAsync(LogMessage message);
}