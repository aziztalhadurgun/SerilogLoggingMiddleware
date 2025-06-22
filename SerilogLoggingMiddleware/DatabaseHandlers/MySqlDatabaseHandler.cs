using System.Data;
using MySql.Data.MySqlClient;
using Serilog;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.DatabaseHandlers;

public class MySqlDatabaseHandler : DatabaseLogHandler
{
    public MySqlDatabaseHandler(string connectionString, string tableName = "Logs")
         : base(connectionString, tableName)
    {
    }

    public override void Configure(LoggerConfiguration loggerConfiguration)
    {
        // Configuration for Serilog's built-in MySQL sink
        loggerConfiguration.WriteTo.MySQL(
            connectionString: _connectionString,
            tableName: _tableName,
            storeTimestampInUtc: true);
    }

    public override async Task HandleLogAsync(LogMessage message)
    {
        await EnsureTableCreatedAsync();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(GetInsertSql(), connection);
        AddParameters(command, message);

        await command.ExecuteNonQueryAsync();
    }

    protected override string GetInsertSql()
    {
        return $@"
                INSERT INTO `{_tableName}` (
                    `Timestamp`, `Level`, `Message`, `Exception`,
                    `CorrelationId`, `TraceId`, `RequestPath`, `HttpMethod`, `StatusCode`, `ElapsedMilliseconds`
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
                CREATE TABLE IF NOT EXISTS `{_tableName}` (
                    `Id` INT AUTO_INCREMENT PRIMARY KEY,
                    `Timestamp` DATETIME NOT NULL,
                    `Level` VARCHAR(50) NOT NULL,
                    `Message` TEXT,
                    `Exception` TEXT,
                    `CorrelationId` VARCHAR(36),
                    `TraceId` VARCHAR(36),
                    `RequestPath` VARCHAR(500),
                    `HttpMethod` VARCHAR(10),
                    `StatusCode` INT,
                    `ElapsedMilliseconds` DOUBLE,
                    INDEX `idx_{_tableName}_timestamp` (`Timestamp`),
                    INDEX `idx_{_tableName}_level` (`Level`),
                    INDEX `idx_{_tableName}_correlationid` (`CorrelationId`),
                    INDEX `idx_{_tableName}_traceid` (`TraceId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
