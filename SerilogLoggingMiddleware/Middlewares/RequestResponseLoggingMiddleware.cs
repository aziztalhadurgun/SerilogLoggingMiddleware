using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
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
        try
        {
            var request = await RequestResponseFormatter.FormatRequest(context.Request);
            request = SensitiveDataMasker.MaskSensitiveData(request);
            _logger.LogInformation("Incoming Request: {Request}", request);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            var response = await RequestResponseFormatter.FormatResponse(context.Response);
            response = SensitiveDataMasker.MaskSensitiveData(response);
            _logger.LogInformation("Outgoing Response: {Response}", response);

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred while executing the request");
            throw;
        }

    }
}