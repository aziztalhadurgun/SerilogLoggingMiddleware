using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SerilogLoggingMiddleware.LogHandlers;

namespace SerilogLoggingMiddleware.Utilities;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration config, IServiceProvider serviceProvider = null)
    {
        // var seqUrl = config["Logging:SeqUrl"];

        // Log.Logger = new LoggerConfiguration()
        //     .MinimumLevel.Information()
        //     .WriteTo.Console()
        //     .WriteTo.Seq(seqUrl)
        //     .CreateLogger();

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

    public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext();

            // Console loglama
            if (configuration.GetValue<bool>("Logging:Handlers:Console:Enabled"))
            {
                loggerConfiguration.WriteTo.Console();
            }

            // Seq loglama
            var seqSection = configuration.GetSection("Logging:Handlers:Seq");
            if (seqSection.GetValue<bool>("Enabled"))
            {
                var seqUrl = seqSection["ServerUrl"];
                if (!string.IsNullOrEmpty(seqUrl))
                {
                    loggerConfiguration.WriteTo.Seq(seqUrl, apiKey: seqSection["ApiKey"]);
                }
            }

            // Diğer log handler'ları...
        });
    }
}
