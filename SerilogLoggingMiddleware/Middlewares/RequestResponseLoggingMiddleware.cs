using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogLoggingMiddleware.Utilities;

namespace SerilogLoggingMiddleware.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IConfiguration config)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context); // pipeline devam etsin

            // Request ve Response logla
            var requestText = await RequestResponseFormatter.FormatRequest(context.Request);
            requestText = SensitiveDataMasker.MaskSensitiveData(requestText);
            _logger.LogInformation("Request: {Request}", requestText);

            var responseText = await RequestResponseFormatter.FormatResponse(context.Response);
            responseText = SensitiveDataMasker.MaskSensitiveData(responseText);
            _logger.LogInformation("Response: {Response}", responseText);

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Middleware Exception");
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream; // her hâlükârda orijinale dön
        }
    }
}