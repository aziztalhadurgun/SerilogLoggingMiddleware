using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using SerilogLoggingMiddleware.Utilities;

namespace SerilogLoggingMiddleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next, string seqUrl)
    {
        _next = next;
        SerilogLoggingConfiguration.ConfigureSerilog(seqUrl);
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

