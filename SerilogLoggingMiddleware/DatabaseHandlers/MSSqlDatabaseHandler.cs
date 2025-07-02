using System.Data;
using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.DatabaseHandlers;

public class MSSqlDatabaseHandler : DatabaseLogHandler
{
    public MSSqlDatabaseHandler(string connectionString, string tableName = "Logs")
        : base(connectionString, tableName) { }

    public override void Configure(LoggerConfiguration loggerConfiguration)
    {
        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = _tableName,
            AutoCreateSqlTable = true,
            BatchPostingLimit = 1000,
            BatchPeriod = TimeSpan.FromSeconds(5)
        };

        var columnOptions = new ColumnOptions
        {
            AdditionalColumns = new List<SqlColumn>
            {
                new SqlColumn
                {
                    ColumnName = "CorrelationId",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 36,
                    AllowNull = true
                },
                new SqlColumn
                {
                    ColumnName = "TraceId",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 36,
                    AllowNull = true
                },
                new SqlColumn
                {
                    ColumnName = "RequestPath",
                    DataType = SqlDbType.NVarChar,
                    AllowNull = true
                },
                new SqlColumn
                {
                    ColumnName = "HttpMethod",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 10,
                    AllowNull = true
                },
                new SqlColumn
                {
                    ColumnName = "StatusCode",
                    DataType = SqlDbType.Int,
                    AllowNull = true
                },
                new SqlColumn
                {
                    ColumnName = "ElapsedMilliseconds",
                    DataType = SqlDbType.Float,
                    AllowNull = true
                }
            }
        };

        loggerConfiguration.WriteTo.MSSqlServer(
            connectionString: _connectionString,
            sinkOptions: sinkOptions,
            columnOptions: columnOptions
        );
    }

    public override async Task HandleLogAsync(LogMessage message)
    {
        await EnsureTableCreatedAsync();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(GetInsertSql(), connection);
        AddParameters(command, message);

        await command.ExecuteNonQueryAsync();
    }

    protected override string GetInsertSql()
    {
        return $@"
                INSERT INTO {_tableName} (
                    Timestamp, Level, Message, Exception,
                    CorrelationId, TraceId, RequestPath, HttpMethod, StatusCode, ElapsedMilliseconds
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
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{_tableName}')
                BEGIN
                    CREATE TABLE [{_tableName}] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Timestamp] DATETIME2 NOT NULL,
                        [Level] NVARCHAR(50) NOT NULL,
                        [Message] NVARCHAR(MAX) NULL,
                        [Exception] NVARCHAR(MAX) NULL,
                        [CorrelationId] NVARCHAR(36) NULL,
                        [TraceId] NVARCHAR(36) NULL,
                        [RequestPath] NVARCHAR(500) NULL,
                        [HttpMethod] NVARCHAR(10) NULL,
                        [StatusCode] INT NULL,
                        [ElapsedMilliseconds] FLOAT NULL
                    );
                    
                    -- Add indexes for better query performance
                    CREATE INDEX IX_{_tableName}_Timestamp ON [{_tableName}] ([Timestamp]);
                    CREATE INDEX IX_{_tableName}_Level ON [{_tableName}] ([Level]);
                    CREATE INDEX IX_{_tableName}_CorrelationId ON [{_tableName}] ([CorrelationId]);
                    CREATE INDEX IX_{_tableName}_TraceId ON [{_tableName}] ([TraceId]);
                END";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

}