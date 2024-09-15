using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace SerilogLoggingMiddleware;

public class RequestResponseLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        Log.Information("Request {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        Log.Information("Response {StatusCode}", context.Response.StatusCode);
    }
}
public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

