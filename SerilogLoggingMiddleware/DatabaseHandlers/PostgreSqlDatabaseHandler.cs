using System.Data;
using Npgsql;
using Serilog;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.DatabaseHandlers;

public class PostgreSqlDatabaseHandler : DatabaseLogHandler
{
    public PostgreSqlDatabaseHandler(string connectionString, string tableName = "logs")
        : base(connectionString, tableName.ToLower()) // PostgreSQL typically uses lowercase table names
    {
    }

    public override void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.PostgreSQL(
            connectionString: _connectionString,
            tableName: _tableName,
            needAutoCreateTable: true);
    }

    public override async Task HandleLogAsync(LogMessage message)
    {
        await EnsureTableCreatedAsync();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(GetInsertSql(), connection);
        AddParameters(command, message);

        await command.ExecuteNonQueryAsync();
    }

    protected override string GetInsertSql()
    {
        return $@"
                INSERT INTO {_tableName} (
                    timestamp, level, message, exception,
                    correlationid, traceid, requestpath, httpmethod, statuscode, elapsedmilliseconds
                ) VALUES (
                    @Timestamp, @Level, @Message, @Exception,
                    @CorrelationId, @TraceId, @RequestPath, @HttpMethod, @StatusCode, @ElapsedMilliseconds
                )";
    }

    protected override void AddParameters(IDbCommand command, LogMessage message)
    {
        AddParameter(command, "@Timestamp", message.Timestamp, DbType.DateTime2);
        AddParameter(command, "@Level", message.Level, DbType.String);
        AddParameter(command, "@Message", message.Message, DbType.String);
        AddParameter(command, "@Exception", message.Exception?.ToString(), DbType.String);
        AddParameter(command, "@CorrelationId", message.CorrelationId, DbType.String);
        AddParameter(command, "@TraceId", message.TraceId, DbType.String);
        AddParameter(command, "@RequestPath", message.RequestPath, DbType.String);
        AddParameter(command, "@HttpMethod", message.HttpMethod, DbType.String);
        AddParameter(command, "@StatusCode", message.StatusCode, DbType.Int32);
        AddParameter(command, "@ElapsedMilliseconds", message.ElapsedMilliseconds, DbType.Double);
    }

    protected override async Task CreateTableIfNotExistsAsync()
    {
        var sql = $@"
                CREATE TABLE IF NOT EXISTS {_tableName} (
                    id SERIAL PRIMARY KEY,
                    timestamp TIMESTAMPTZ NOT NULL,
                    level VARCHAR(50) NOT NULL,
                    message TEXT,
                    exception TEXT,
                    correlationid VARCHAR(36),
                    traceid VARCHAR(36),
                    requestpath VARCHAR(500),
                    httpmethod VARCHAR(10),
                    statuscode INTEGER,
                    elapsedmilliseconds DOUBLE PRECISION
                );

                -- Create indexes if they don't exist
                CREATE INDEX IF NOT EXISTS idx_{_tableName}_timestamp ON {_tableName} (timestamp);
                CREATE INDEX IF NOT EXISTS idx_{_tableName}_level ON {_tableName} (level);
                CREATE INDEX IF NOT EXISTS idx_{_tableName}_correlationid ON {_tableName} (correlationid);
                CREATE INDEX IF NOT EXISTS idx_{_tableName}_traceid ON {_tableName} (traceid);";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
