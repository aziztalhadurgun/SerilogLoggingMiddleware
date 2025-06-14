using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using SerilogLoggingMiddleware.Utilities;

namespace SerilogLoggingMiddleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        SerilogLoggingConfiguration.ConfigureSerilog(configuration);
    }

    public async Task Invoke(HttpContext context)
    {
        // var request = await RequestResponseFormatter.FormatRequest(context.Request);
        // request = SensitiveDataMasker.MaskSensitiveData(request);
        // Log.Information("Incoming Request: {Request}", request);

        // var originalBodyStream = context.Response.Body;

        // using var responseBody = new MemoryStream();
        // context.Response.Body = responseBody;

        // await _next(context);

        // var response = await RequestResponseFormatter.FormatResponse(context.Response);
        // response = SensitiveDataMasker.MaskSensitiveData(response);
        // Log.Information("Outgoing Response: {Response}", response);

        // await responseBody.CopyToAsync(originalBodyStream);


         // Request verisini yakala
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        var requestData = new HttpRequestData
        {
            Body = requestBody,
            Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        };

        Log.Information("Request: {@RequestData}", requestData);

        // Response'u yakalamak için stream kullan
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Response verisini yakala
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseData = new HttpResponseData
        {
            Body = responseBodyText,
            Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
        };

        Log.Information("Response: {@ResponseData}", responseData);

        await responseBody.CopyToAsync(originalBodyStream);

    }
}

public class HttpRequestData
{
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}

public class HttpResponseData
{
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, IConfiguration configuration)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>(configuration);
    }
}

