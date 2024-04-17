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
        var corsPolicySection = _configuration.GetSection("CorsPolicy");
        var allowedOrigins = corsPolicySection.GetSection("AllowedOrigins").Get<string[]>() ?? System.Array.Empty<string>();
        var origin = context.Request.Headers["Origin"].ToString();

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.Headers["Access-Control-Max-Age"] = "86400"; // Preflight cache duration
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        }

        if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = origin;
            context.Response.Headers["Access-Control-Allow-Methods"] = string.Join(", ", corsPolicySection.GetSection("AllowedMethods").Get<string[]>() ?? System.Array.Empty<string>());
            context.Response.Headers["Access-Control-Allow-Headers"] = string.Join(", ", corsPolicySection.GetSection("AllowedHeaders").Get<string[]>() ?? System.Array.Empty<string>());

            if (corsPolicySection.GetValue<bool>("AllowCredentials"))
            {
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            }

        }
        await _next(context);
    }
}
