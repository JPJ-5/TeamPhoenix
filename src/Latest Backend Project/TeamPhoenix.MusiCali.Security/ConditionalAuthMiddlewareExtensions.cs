using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

public static class ConditionalAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseConditionalAuth(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.Use(async (context, next) =>
        {
            var anonymousPaths = configuration.GetSection("AllowedAnonymousEndpoints").Get<List<string>>();
            if (anonymousPaths.Contains(context.Request.Path.ToString()))
            {
                // If the path is in the anonymous list, simply continue to the next middleware
                await next();
            }
            else
            {
                // If not, execute the Authentication and Authorization middlewares

                // Ensure your middleware handles calling `next()` internally if needed
                await new AuthenticationMiddleware(next, configuration).Invoke(context);
                await new AuthorizationMiddleware(next, configuration).Invoke(context);
            }
        });

        return app;
    }
}
