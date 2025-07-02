using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogLoggingMiddleware.Handlers;
using SerilogLoggingMiddleware.LogHandlers;
using SerilogLoggingMiddleware.Middlewares;
using SerilogLoggingMiddleware.Models;
using SerilogLoggingMiddleware.Utilities;

namespace SerilogLoggingMiddleware;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures Serilog logging with the specified configuration
    /// </summary>
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration,  Action<LogHandlerOptions> configureOptions = null)
    {
        var provider = services.BuildServiceProvider();
        SerilogLoggingConfiguration.ConfigureSerilog(configuration, provider);

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        });


        services.AddSingleton<LogHandlerFactory>(provider =>
            new LogHandlerFactory(configuration, provider.GetService<ILoggerFactory>()));

        if (configureOptions != null)
        {
            var options = new LogHandlerOptions();
            configureOptions(options);
            services.AddSingleton(options);
        }

        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdDelegatingHandler>();

        return services;
    }

    /// <summary>
    /// Adds correlation ID support for HTTP clients
    /// </summary>
    public static IHttpClientBuilder AddCorrelationIdHttpClient(this IServiceCollection services, string name, Action<HttpClient> configureClient = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("HTTP client name cannot be null or empty", nameof(name));

        var builder = services.AddHttpClient(name, configureClient);
        builder.AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
        return builder;
    }

    /// <summary>
    /// Adds correlation ID middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Adds request/response logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}