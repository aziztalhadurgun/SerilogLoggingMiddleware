using Serilog;

namespace SerilogLoggingMiddleware.Utilities;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(string seqUrl)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();
    }
}
