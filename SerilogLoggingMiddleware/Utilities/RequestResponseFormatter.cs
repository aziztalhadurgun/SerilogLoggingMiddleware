using System.Text;
using Microsoft.AspNetCore.Http;

namespace SerilogLoggingMiddleware.Utilities;

public static class RequestResponseFormatter
{
    public static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return $"Method:{request.Method} => Path: {request.Path} => Body: {bodyAsText}";
    }

    public static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"StatusCode: {response.StatusCode} => Body: {text}";
    }
}