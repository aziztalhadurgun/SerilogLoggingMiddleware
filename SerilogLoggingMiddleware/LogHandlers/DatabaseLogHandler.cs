using System.Data;
using Serilog;
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
}