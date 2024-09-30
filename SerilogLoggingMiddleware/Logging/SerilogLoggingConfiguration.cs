using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SerilogLoggingMiddleware.LogHandlers;

namespace SerilogLoggingMiddleware.Logging;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration config)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(config);

        var minimumLevel = config["Logging:MinimumLevel:Default"];
        if (!string.IsNullOrWhiteSpace(minimumLevel) && Enum.TryParse(minimumLevel, true, out LogEventLevel logLevel))
        {
            loggerConfig.MinimumLevel.Is(logLevel);
        }
        else
        {
            loggerConfig.MinimumLevel.Information();
        }

        var writeToOptions = config.GetSection("Logging:WriteTo").Get<List<WriteToOption>>();

        if (writeToOptions != null)
        {
            foreach (var option in writeToOptions)
            {
                if (string.IsNullOrWhiteSpace(option.Name) )
                {
                    throw new ArgumentException("Invalid logging configuration.");
                }

                var handler = LogHandlerFactory.Create(option.Name);
                loggerConfig = handler.Handle(loggerConfig, option.Args);
            }
        }

        Log.Logger = loggerConfig.CreateLogger();
    }   
}
