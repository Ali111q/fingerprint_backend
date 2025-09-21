using Microsoft.AspNetCore.Http;

namespace FingerPrintVerfication.Middleware;

public class CorsMiddleware
{
    private readonly RequestDelegate _next;

    public CorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add CORS headers to ALL responses
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, PATCH");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept, Origin");
        context.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition, Content-Length, Content-Type");
        context.Response.Headers.Add("Access-Control-Max-Age", "86400");

        // Handle preflight OPTIONS requests
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        await _next(context);
    }
}
