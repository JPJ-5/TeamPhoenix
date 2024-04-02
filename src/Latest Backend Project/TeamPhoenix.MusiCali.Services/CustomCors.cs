using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamPhoenix.MusiCali.DataAccessLayer;

public static class CustomCorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder builder)
    {
        var configuration = builder.ApplicationServices.GetRequiredService<IConfiguration>();
        return builder.UseMiddleware<CustomCorsMiddleware>(configuration);
    }
}