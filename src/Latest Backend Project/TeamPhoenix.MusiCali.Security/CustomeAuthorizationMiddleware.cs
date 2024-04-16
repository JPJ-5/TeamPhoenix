using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        // This is the Access Token
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (accessToken == null)
        {
            var path = context.Request.Path;

            // Check if the path matches your criteria
            if (_configuration.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>().Contains(path))
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
        if (accessToken != null && HasRequiredPermissions(accessToken, context))
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

    private bool HasRequiredPermissions(string accessToken, HttpContext context)
    {
        var parts = accessToken.Split('.');
        if (parts.Length != 3)
            return false;

        var header = parts[0];
        var payload = parts[1];
        var signature = parts[2];
        var computedSignature = ComputeHmacSha256(header + "." + payload, "simple-key");
        
        var path = context.Request.Path;


        var decodedPayload = Base64UrlDecode(payload);


        using JsonDocument doc = JsonDocument.Parse(decodedPayload);
        JsonElement root = doc.RootElement;

        string audience = root.GetProperty("aud").GetString()!;
        string scope = root.GetProperty("scope").GetString()!;
        //Console.WriteLine(scope);


        if (signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase)
            && _configuration.GetSection("Jwt:Audience").Value!.Equals(audience)
            && scope == "NormalUser"
            && _configuration.GetSection("AllowedEndpoints:Normal").Get<List<string>>().Contains(path)
           )
        {
            return true;
        }
        if (signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase)
            && _configuration.GetSection("Jwt:Audience").Value!.Equals(audience)
            && scope == "AdminUser"
            && (_configuration.GetSection("AllowedEndpoints:Admin").Get<List<string>>().Contains(path))
           )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private string ComputeHmacSha256(string text, string key)
    {
        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            // Encode using Base64
            return Convert.ToBase64String(hashmessage).TrimEnd('=')
                .Replace('+', '-').Replace('/', '_');
        }
    }

    private static string Base64UrlDecode(string input)
    {

        var bytes = WebEncoders.Base64UrlDecode(input);

        return Encoding.UTF8.GetString(bytes);
    }
}