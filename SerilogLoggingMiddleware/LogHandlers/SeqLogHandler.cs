using Serilog;

namespace SerilogLoggingMiddleware.LogHandlers;

public class SeqLogHandler : ILogHandler
{
    public LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args)
    {
        if (args.TryGetValue("serverUrl", out var serverUrl))
        {
            return loggerConfig.WriteTo.Seq(serverUrl);
        }
        throw new ArgumentException("Seq server URL must be provided.");
    }
}