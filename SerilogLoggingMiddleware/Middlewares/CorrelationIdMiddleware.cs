using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace SerilogLoggingMiddleware.Middlewares;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        string correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var cid) 
            ? cid.ToString() 
            : Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        LogContext.PushProperty("CorrelationId", correlationId);
        await _next(context);
    }
}