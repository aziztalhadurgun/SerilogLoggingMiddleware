using Serilog;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public class SeqLogHandler : ILogHandler
{
    private readonly ILogger _logger;
    private readonly string _seqServerUrl;
    private readonly string _apiKey;

    public SeqLogHandler(string seqServerUrl, string apiKey)
    {
        _seqServerUrl = seqServerUrl;
        _apiKey = apiKey;
        _logger = new LoggerConfiguration()
            .WriteTo.Seq(_seqServerUrl, apiKey: _apiKey)
            .CreateLogger()
            .ForContext<SeqLogHandler>();
    }

    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.Seq(_seqServerUrl, apiKey: _apiKey);
    }

    public async Task HandleLogAsync(LogMessage message)
    {
        var logger = _logger
            .ForContext("CorrelationId", message.CorrelationId)
            .ForContext("TraceId", message.TraceId)
            .ForContext("RequestPath", message.RequestPath)
            .ForContext("HttpMethod", message.HttpMethod)
            .ForContext("StatusCode", message.StatusCode)
            .ForContext("ElapsedMilliseconds", message.ElapsedMilliseconds);

        switch (message.Level.ToUpper())
        {
            case "ERROR":
                logger.Error(message.Exception, message.Message);
                break;
            case "WARNING":
                logger.Warning(message.Message);
                break;
            case "INFO":
                logger.Information(message.Message);
                break;
            case "DEBUG":
                logger.Debug(message.Message);
                break;
            default:
                logger.Information(message.Message);
                break;
        }
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        (_logger as IDisposable)?.Dispose();
    }
}