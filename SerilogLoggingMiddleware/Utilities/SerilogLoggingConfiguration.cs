using Microsoft.Extensions.Configuration;
using Serilog;

namespace SerilogLoggingMiddleware.Utilities;

public static class SerilogLoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .CreateLogger();
    }
}
