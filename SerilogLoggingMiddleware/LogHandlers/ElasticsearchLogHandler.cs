using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace SerilogLoggingMiddleware.LogHandlers;

public class ElasticsearchLogHandler : ILogHandler
{
    public LoggerConfiguration Handle(LoggerConfiguration loggerConfig, Dictionary<string, string> args)
    {
        if (args.TryGetValue("nodeUris", out var nodeUris) &&
            args.TryGetValue("indexFormat", out var indexFormat))
        {
            return loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(nodeUris))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat
            });
        }
        throw new ArgumentException("Elasticsearch nodeUris and indexFormat must be provided.");
    }
}