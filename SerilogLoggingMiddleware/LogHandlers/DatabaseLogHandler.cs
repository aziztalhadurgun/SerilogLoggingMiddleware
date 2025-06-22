using System.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.LogHandlers;

public abstract class DatabaseLogHandler : ILogHandler, IDisposable
{

    protected readonly string _connectionString;
    protected readonly string _tableName;
    private bool _disposed = false;
    private bool _tableCreated = false;

    protected DatabaseLogHandler(string connectionString, string tableName = "Logs")
    {
        _connectionString = connectionString;
        _tableName = tableName;
    }

    public abstract void Configure(LoggerConfiguration loggerConfiguration);
    public abstract Task HandleLogAsync(LogMessage message);
    protected abstract Task CreateTableIfNotExistsAsync();

    protected abstract string GetInsertSql();

    protected abstract void AddParameters(IDbCommand command, LogMessage message);

    protected virtual void AddParameter(IDbCommand command, string name, object value, DbType dbType)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        parameter.DbType = dbType;
        command.Parameters.Add(parameter);
    }

    public async Task EnsureTableCreatedAsync()
    {
        if (!_tableCreated)
        {
            await CreateTableIfNotExistsAsync();
            _tableCreated = true;
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
                // Dispose managed resources
            }
            _disposed = true;
        }
    }

    ~DatabaseLogHandler()
    {
        Dispose(false);
    }

    // private readonly string _connectionString;
    // private readonly string _databaseType;
    // private readonly string _tableName;

    // public DatabaseLogHandler(string connectionString, string databaseType, string tableName = "Logs")
    // {
    //     _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    //     _databaseType = databaseType?.ToUpper() ?? throw new ArgumentNullException(nameof(databaseType));
    //     _tableName = tableName;
    // }

    // public void Configure(LoggerConfiguration loggerConfiguration)
    // {
    //     switch (_databaseType)
    //     {
    //         case "MSSQL":
    //             loggerConfiguration.WriteTo.MSSqlServer(
    //                 connectionString: _connectionString,
    //                 sinkOptions: new MSSqlServerSinkOptions { TableName = _tableName });
    //             break;
    //         case "POSTGRESQL":
    //             loggerConfiguration.WriteTo.PostgreSQL(
    //                 connectionString: _connectionString,
    //                 tableName: _tableName);
    //             break;
    //         case "MYSQL":
    //             loggerConfiguration.WriteTo.MySQL(
    //                 connectionString: _connectionString,
    //                 tableName: _tableName);
    //             break;
    //         case "MONGODB":
    //             loggerConfiguration.WriteTo.MongoDB(
    //                 databaseUrl: _connectionString,
    //                 collectionName: _tableName);
    //             break;
    //         default:
    //             throw new NotSupportedException($"Database type '{_databaseType}' is not supported.");
    //     }
    // }

    // public Task HandleLogAsync(LogMessage message)
    // {
    //     // Database logging is handled by Serilog's sinks, so we don't need to implement this
    //     return Task.CompletedTask;
    // }
}