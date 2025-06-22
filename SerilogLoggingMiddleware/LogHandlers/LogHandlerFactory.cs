using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogLoggingMiddleware.DatabaseHandlers;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public class LogHandlerFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly List<ILogHandler> _handlers = new();
    private bool _disposed = false;

    public LogHandlerFactory(IConfiguration configuration, ILoggerFactory loggerFactory = null)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _loggerFactory = loggerFactory;
    }

    public IEnumerable<ILogHandler> CreateHandlers()
    {
        if (_handlers.Any())
            return _handlers;

        var loggingSection = _configuration.GetSection("Logging:Handlers");

        // Console Handler
        if (loggingSection.GetValue<bool>("Console:Enabled"))
        {
            _handlers.Add(new ConsoleLogHandler());
        }


        // Seq Handler
        var seqSection = loggingSection.GetSection("Seq");
        if (seqSection.GetValue<bool>("Enabled"))
        {
            var seqUrl = seqSection["ServerUrl"];
            var apiKey = seqSection["ApiKey"];

            if (string.IsNullOrEmpty(seqUrl))
            {
                throw new InvalidOperationException("Seq server URL is required when Seq logging is enabled");
            }

            _handlers.Add(new SeqLogHandler(seqUrl, apiKey));
        }

        // File Handler
        var fileSection = loggingSection.GetSection("File");
        if (fileSection.GetValue<bool>("Enabled"))
        {
            var filePath = fileSection["Path"];
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path is required for file logging");
            }
            _handlers.Add(new FileLogHandler(filePath));
        }

        // Database Handler
        var dbSection = loggingSection.GetSection("Database");
        if (dbSection.GetValue<bool>("Enabled"))
        {
            var connectionString = dbSection["ConnectionString"];
            var databaseType = dbSection["DatabaseType"];
            var tableName = dbSection["TableName"] ?? "Logs";

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is required for database logging");
            }

            if (string.IsNullOrEmpty(databaseType))
            {
                throw new InvalidOperationException("DatabaseType is required for database logging");
            }

            ILogHandler handler = databaseType switch
            {
                "MSSQL" => new MSSqlDatabaseHandler(connectionString, tableName),
                "POSTGRESQL" => new PostgreSqlDatabaseHandler(connectionString, tableName),
                "MYSQL" => new MySqlDatabaseHandler(connectionString, tableName),
                "MONGODB" => new MongoDbDatabaseHandler(connectionString, tableName),
                _ => throw new NotSupportedException($"Database type '{databaseType}' is not supported")
            };

            _handlers.Add(handler);
        }

        return _handlers;
    }

    public void ConfigureAll(LoggerConfiguration loggerConfiguration)
    {
        if (loggerConfiguration == null)
            throw new ArgumentNullException(nameof(loggerConfiguration));

        foreach (var handler in _handlers)
        {
            try
            {
                handler.Configure(loggerConfiguration);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the application
                Console.WriteLine($"Error configuring handler {handler.GetType().Name}: {ex.Message}");
            }
        }
    }

    public async Task HandleLogAsync(LogMessage message)
    {
        if (message == null) return;

        var tasks = _handlers
            .Select(handler => SafeHandleLogAsync(handler, message))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task SafeHandleLogAsync(ILogHandler handler, LogMessage message)
    {
        try
        {
            await handler.HandleLogAsync(message);
        }
        catch (Exception ex)
        {
            _loggerFactory?.CreateLogger(handler.GetType())
                    .LogError(ex, "Error in {HandlerType} while processing log", handler.GetType().Name);
            Console.WriteLine($"Error in {handler.GetType().Name}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var handler in _handlers.OfType<IDisposable>())
                {
                    try
                    {
                        handler.Dispose();
                    }
                    catch
                    {
                        // Suppress exceptions during disposal
                    }
                }
                _handlers.Clear();
            }
            _disposed = true;
        }
    }
}