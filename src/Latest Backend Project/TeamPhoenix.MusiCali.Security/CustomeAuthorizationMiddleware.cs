using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contracts;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthorization authorizationService)
    {
        // Retrieve user principal from context (set by AuthenticationMiddleware)
        var username = context.Items["User"] as string;
        if (username == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied");
            return;
        }

        // You might want to resolve the resource and action from the request
        string resource = ""; // Determine based on context.Request
        string action = ""; // Determine based on context.Request

        Principal userPrincipal = new Principal(username, new Dictionary<string, string>()); // Get from somewhere or fill with real data

        if (!authorizationService.IsUserAuthorized(userPrincipal, resource, action))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("User is not authorized to perform this action");
            return;
        }

        await _next(context);
    }
}
