using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
public class CustomCorsMiddleware
{
    private readonly RequestDelegate _next;
    private IConfiguration _configuration;

    public CustomCorsMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract CORS configuration
        var corsPolicySection = _configuration.GetSection("CorsPolicy");
        var allowedOrigins = corsPolicySection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var origin = context.Request.Headers["Origin"].ToString();

        // Preflight request handling
        if (context.Request.Method == "OPTIONS")
        {
            // Check origin validity
            if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
            {
                SetCorsHeaders(context, corsPolicySection, origin);
                context.Response.Headers["Access-Control-Max-Age"] = "86400"; // Preflight cache duration
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return; // Terminate pipeline for OPTIONS requests
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return; // Terminate pipeline if origin is not allowed
            }
        }

        // Non-preflight CORS handling
        if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
        {
            SetCorsHeaders(context, corsPolicySection, origin);
        }

        await _next(context);
    }

    private void SetCorsHeaders(HttpContext context, IConfigurationSection corsPolicySection, string origin)
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        context.Response.Headers["Access-Control-Allow-Methods"] = string.Join(", ", corsPolicySection.GetSection("AllowedMethods").Get<string[]>() ?? Array.Empty<string>());
        context.Response.Headers["Access-Control-Allow-Headers"] = string.Join(", ", corsPolicySection.GetSection("AllowedHeaders").Get<string[]>() ?? Array.Empty<string>());

        if (corsPolicySection.GetValue<bool>("AllowCredentials"))
        {
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        }
    }
}