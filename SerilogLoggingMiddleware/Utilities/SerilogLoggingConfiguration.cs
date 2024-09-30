using Microsoft.Extensions.Configuration;
using Serilog;

namespace SerilogLoggingMiddleware.Utilities;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration config)
    {
        var seqUrl = config["Logging:SeqUrl"];

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();
    }
}
