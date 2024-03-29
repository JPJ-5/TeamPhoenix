using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNetCore.Http.Extensions;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Configuration? _configuration;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authentication"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
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
                // Call the next middleware in the pipeline
                context.Response.StatusCode = 401; // Unauthenticated
                return;
            }
        }

        if (!ValidateToken(token))
        {
            context.Response.StatusCode = 401; // Unauthenticated
            return;
        }

        await _next(context);
    }

    private bool ValidateToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return false;

            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var computedSignature = ComputeHmacSha256(header + "." + payload, "simple-key");
            return signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private string ComputeHmacSha256(string text, string key)
    {
        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hashmessage).TrimEnd('=')
                .Replace('+', '-').Replace('/', '_'); // Base64Url Encoding
        }
    }
}