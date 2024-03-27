using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TeamPhoenix.MusiCali.Security.Contracts;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Bypass middleware logic for GET requests or specific paths immediately
        if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }
        else if (context.Request.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }
        else if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }
        else if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }
        else if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }
    }
}