using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SerilogLoggingMiddleware.LogHandlers;

namespace SerilogLoggingMiddleware.Utilities;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration config, IServiceProvider serviceProvider = null)
    {
        var loggerConfig = new LoggerConfiguration()
           .MinimumLevel.Information()
           .Enrich.FromLogContext();

        // LogHandlerFactory'yi kullanarak handler'ları oluştur
        var logHandlerFactory = serviceProvider?.GetService<LogHandlerFactory>() ??
                              new LogHandlerFactory(config);

        var handlers = logHandlerFactory.CreateHandlers();
        foreach (var handler in handlers)
        {
            handler.Configure(loggerConfig);
        }

        Log.Logger = loggerConfig.CreateLogger();
    }

}
