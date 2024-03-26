using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthentication authenticationService)
    {
        string? username = context.Request.Headers["Username"];
        string? otp = context.Request.Headers["OTP"];

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(otp))
        {
            try
            {
                var tokens = authenticationService.Authenticate(username, otp);
                if (tokens.Any())
                {
                    // Assuming the presence of tokens indicates successful authentication
                    context.Items["User"] = username; // or whatever user principal you wish to attach
                    await _next(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                // Log exception and potentially set context.Response to indicate an error
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authentication failed");
                return;
            }
        }

        // If no username or OTP provided, or authentication fails
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Authentication credentials were not provided or are invalid");
    }
}
