using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // This could be the same ID token or a different Access Token depending on your design
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null || !HasRequiredPermissions(token))
        {
            var fullUrl = context.Request.GetDisplayUrl();

            // Parse the URL to get the path
            var uri = new Uri(fullUrl);
            var path = uri.AbsolutePath;
            Console.WriteLine(path);

            // Check if the path matches your criteria
            if (path.StartsWith("/Login/api/CheckUsernameAPI") || path.StartsWith("/Login/api/GetJwtAPI") || path.StartsWith("/AccCreationAPI/api/NormalAccCreationAPI"))
            {
                await _next(context);
                return;
            }
            else
            {
                context.Response.StatusCode = 403; // Forbidden
                return;
            }
        }

        await _next(context);
    }

    private bool HasRequiredPermissions(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1];
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

        // Parse the JSON to get the role or permissions
        // This is a very basic way to parse JSON. Consider using a JSON library.
        var permissions = new Regex("\"permissions\": \"([^\"]+)\"").Match(payloadJson).Groups[1].Value;

        // Check if the permissions include what's needed
        return permissions.Contains("requiredPermission");
    }
    private static byte[] Base64UrlDecode(string input)
    {
        string output = input;
        output = output.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 0: break;
            case 2: output += "=="; break;
            case 3: output += "="; break;
            default: throw new FormatException("Illegal base64url string!");
        }
        return Convert.FromBase64String(output);
    }
}