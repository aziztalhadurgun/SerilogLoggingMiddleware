namespace SerilogLoggingMiddleware.LogHandlers;

public static class LogHandlerFactory
{
    private static readonly Dictionary<string, Func<ILogHandler>> handlers = new Dictionary<string, Func<ILogHandler>>(StringComparer.OrdinalIgnoreCase)
    {
        { "CONSOLE", () => new ConsoleLogHandler() },
        { "FILE", () => new FileLogHandler() },
        { "SEQ", () => new SeqLogHandler() },
        { "ELASTICSEARCH", () => new ElasticsearchLogHandler() },
        { "DATABASE", () => new DatabaseLogHandler() }
    };

    public static ILogHandler Create(string name)
    {
        if (handlers.TryGetValue(name, out var handlerFactory))
        {
            return handlerFactory();
        }
        throw new NotSupportedException($"Log target '{name}' is not supported.");
    }
}