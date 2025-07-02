using Serilog;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public interface ILogHandler
{
    void Configure(LoggerConfiguration loggerConfiguration);
    Task HandleLogAsync(LogMessage message);
}