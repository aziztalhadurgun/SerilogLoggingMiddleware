using Serilog;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public class ConsoleLogHandler : ILogHandler
{
    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message} [TraceId: {TraceId}] [CorrelationId: {CorrelationId}] [RequestPath: {RequestPath}] [HttpMethod: {HttpMethod}] [StatusCode: {StatusCode}] [ElapsedMs: {ElapsedMilliseconds}]{NewLine}{Exception}");
    }

    public async Task HandleLogAsync(LogMessage message)
    {
        var logText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{message.Level}] {message.Message}";

        Console.ForegroundColor = message.Level.ToUpper() switch
        {
            "ERROR" => ConsoleColor.Red,
            "WARNING" => ConsoleColor.Yellow,
            "INFO" => ConsoleColor.Green,
            _ => ConsoleColor.White,
        };
        await Task.Run(() => Console.WriteLine(logText));
        Console.ResetColor();
    }
}