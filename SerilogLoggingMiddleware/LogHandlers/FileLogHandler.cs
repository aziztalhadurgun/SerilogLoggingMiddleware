using Serilog;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public class FileLogHandler : ILogHandler
{
    private readonly string _filePath;

    public FileLogHandler(string filePath)
    {
        _filePath = filePath;
        CreateDirectoryIfNotExists();
    }

    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.File(_filePath,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message} [TraceId: {TraceId}] [CorrelationId: {CorrelationId}] [RequestPath: {RequestPath}] [HttpMethod: {HttpMethod}] [StatusCode: {StatusCode}] [ElapsedMs: {ElapsedMilliseconds}]{NewLine}{Exception}");
    }

    private void CreateDirectoryIfNotExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task HandleLogAsync(LogMessage message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{message.Level}] {message.Message}{Environment.NewLine}";
        
        await File.AppendAllTextAsync(_filePath, logEntry);
    }
}