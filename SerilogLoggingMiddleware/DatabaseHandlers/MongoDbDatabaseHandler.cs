using System.Data;
using MongoDB.Driver;
using Serilog;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware.DatabaseHandlers;

public class MongoDbDatabaseHandler : DatabaseLogHandler
{
    private readonly IMongoDatabase _database;
    private readonly string _collectionName;

    public MongoDbDatabaseHandler(string connectionString, string collectionName = "logs")
        : base(connectionString, collectionName) // For MongoDB, tableName is used as collectionName
    {
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        _database = client.GetDatabase(mongoUrl.DatabaseName ?? "Logs");
        _collectionName = collectionName.ToLower(); // MongoDB typically uses lowercase collection names
    }

    public override void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.MongoDB(
            databaseUrl: _connectionString,
            collectionName: _collectionName);
    }

    public override async Task HandleLogAsync(LogMessage message)
    {
        try
        {
            var collection = _database.GetCollection<LogDocument>(_collectionName);
            var logDoc = new LogDocument
            {
                Timestamp = message.Timestamp,
                Level = message.Level,
                Message = message.Message,
                Exception = message.Exception?.ToString(),
                CorrelationId = message.CorrelationId,
                TraceId = message.TraceId,
                RequestPath = message.RequestPath,
                HttpMethod = message.HttpMethod,
                StatusCode = message.StatusCode,
                ElapsedMilliseconds = message.ElapsedMilliseconds
            };

            await collection.InsertOneAsync(logDoc);
        }
        catch (Exception ex)
        {
            // Log the error but don't throw to prevent application crashes
            Console.WriteLine($"Error writing to MongoDB: {ex.Message}");
        }
    }

    protected override string GetInsertSql()
    {
        // Not used for MongoDB
        return string.Empty;
    }

    protected override void AddParameters(IDbCommand command, LogMessage message)
    {
        // Not used for MongoDB
    }

    protected override Task CreateTableIfNotExistsAsync()
    {
        // MongoDB creates collections automatically on first insert
        // We'll create an index for better query performance
        try
        {
            var collection = _database.GetCollection<LogDocument>(_collectionName);
            var indexKeys = Builders<LogDocument>.IndexKeys
                .Ascending(x => x.Timestamp)
                .Ascending(x => x.Level)
                .Ascending(x => x.CorrelationId)
                .Ascending(x => x.TraceId);

            var indexOptions = new CreateIndexOptions { Name = "query_index" };
            var indexModel = new CreateIndexModel<LogDocument>(indexKeys, indexOptions);

            collection.Indexes.CreateOne(indexModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating MongoDB index: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}