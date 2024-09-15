using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace SerilogLoggingMiddleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Gelen HTTP isteği bilgilerini logla
        Log.Information("Request {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context); // Bir sonraki middleware'e geç

        // Yanıt bilgilerini logla
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

