using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerilogLoggingMiddleware.Handlers;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Middlewares;
using SerilogLoggingMiddleware.Models;

namespace SerilogLoggingMiddleware;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSerilogLogging(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddSingleton<LogHandlerFactory>(provider =>
            new LogHandlerFactory(configuration));

        return services;
    }

    // public static IServiceCollection AddSerilogLogging(
    //     this IServiceCollection services,
    //     Action<LogHandlerOptions> configureOptions)
    // {
    //     var options = new LogHandlerOptions();
    //     configureOptions(options);

    //     services.Configure<LogHandlerOptions>(config =>
    //     {
    //         config.EnableConsole = options.EnableConsole;
    //         config.EnableDatabase = options.EnableDatabase;
    //         config.EnableSeq = options.EnableSeq;
    //         config.EnableFile = options.EnableFile;
    //         config.DatabaseConnectionString = options.DatabaseConnectionString;
    //         config.FilePath = options.FilePath;
    //         config.SeqServerUrl = options.SeqServerUrl;
    //         config.SeqApiKey = options.SeqApiKey;
    //     });

    //     services.AddSingleton<LogHandlerFactory>();
    //     services.AddTransient<RequestResponseLoggingMiddleware>();

    //     return services;
    // }

    public static IServiceCollection AddCorrelationIdHttpClientSupport(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdDelegatingHandler>();
        return services;
    }

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationIdMiddleware>();

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestResponseLoggingMiddleware>();


}