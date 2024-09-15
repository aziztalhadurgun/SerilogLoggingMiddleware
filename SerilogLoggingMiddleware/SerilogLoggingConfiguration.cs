using Serilog;

namespace SerilogLoggingMiddleware;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(string seqUrl)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()  // Logları konsola yazdır
            .WriteTo.Seq(seqUrl)  // Logları Seq'e gönder
            .CreateLogger();
    }
}
