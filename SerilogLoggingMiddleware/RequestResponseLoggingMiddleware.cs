using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogLoggingMiddleware.Utilities;

namespace SerilogLoggingMiddleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IConfiguration config)
    {
        _next = next;
        _logger = logger;
        SerilogLoggingConfiguration.ConfigureSerilog(config);
    }

    public async Task Invoke(HttpContext context)
    {
        var request = await RequestResponseFormatter.FormatRequest(context.Request);
        request = SensitiveDataMasker.MaskSensitiveData(request);
        Log.Information("Incoming Request: {Request}", request);

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        var response = await RequestResponseFormatter.FormatResponse(context.Response);
        response = SensitiveDataMasker.MaskSensitiveData(response);
        Log.Information("Outgoing Response: {Response}", response);

        await responseBody.CopyToAsync(originalBodyStream);
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, string seqUrl)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>(seqUrl);
    }
}

