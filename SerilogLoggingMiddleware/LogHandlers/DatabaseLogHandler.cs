using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace SerilogLoggingMiddleware.LogHandlers;

public class DatabaseLogHandler : ILogHandler
{
    public LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args)
    {
        if (!args.TryGetValue("databaseType", out var databaseType) ||
            !args.TryGetValue("connectionString", out var connectionString))
        {
            throw new ArgumentException("DatabaseType and ConnectionString must be provided for database logging.");
        }

        var tableName = args.ContainsKey("tableName") ? args["tableName"] : "Logs";

        switch (databaseType.ToUpper())
        {
            case "MSSQL":
                return loggerConfig.WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = tableName });
            case "POSTGRESQL":
                return loggerConfig.WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: tableName);
            case "MYSQL":
                return loggerConfig.WriteTo.MySQL(
                    connectionString: connectionString,
                    tableName: tableName);
            case "MONGODB":
                return loggerConfig.WriteTo.MongoDB(
                    databaseUrl: connectionString,
                    collectionName: tableName);
            default:
                throw new NotSupportedException($"Database type '{databaseType}' is not supported.");
        }
    }
}