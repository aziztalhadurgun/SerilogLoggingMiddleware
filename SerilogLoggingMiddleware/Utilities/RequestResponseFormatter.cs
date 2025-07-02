using Microsoft.AspNetCore.Http;

namespace SerilogLoggingMiddleware.Utilities;

public static class RequestResponseFormatter
{
     public static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);

        return $"Method: {request.Method}, Path: {request.Path}, Body: {body}";
    }
    
    public static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"StatusCode: {response.StatusCode} => Body: {text}";
    }
}