using Serilog;
using Serilog.Core;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public class SeqLogHandler : ILogHandler
{
    private readonly Logger _logger;
    private readonly string _seqServerUrl;
    private readonly string _apiKey;

    public SeqLogHandler(string seqServerUrl, string apiKey)
    {
        _seqServerUrl = seqServerUrl;
        _apiKey = apiKey;
    }


    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.Seq(_seqServerUrl, apiKey: _apiKey);
    }

    public async Task HandleLogAsync(LogMessage message)
    {
        await Task.Run(() =>
        {
            switch (message.Level.ToUpper())
            {
                case "ERROR":
                    _logger.Error(message.Message);
                    break;
                case "WARNING":
                    _logger.Warning(message.Message);
                    break;
                case "INFO":
                    _logger.Information(message.Message);
                    break;
                case "DEBUG":
                    _logger.Debug(message.Message);
                    break;
                default:
                    _logger.Information(message.Message);
                    break;
            }
        });
    }

    public void Dispose()
    {
        _logger?.Dispose();
    }
}