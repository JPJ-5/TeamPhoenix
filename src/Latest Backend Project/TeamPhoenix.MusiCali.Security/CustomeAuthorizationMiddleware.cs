using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
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
        // This could be the same ID token or a different Access Token depending on your design
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (accessToken == null/* || !HasRequiredPermissions(accessToken, context)*/)
        {
            var fullUrl = context.Request.GetDisplayUrl();

            // Parse the URL to get the path
            var uri = new Uri(fullUrl);
            var path = uri.AbsolutePath;
            //Console.WriteLine(path);

            // Check if the path matches your criteria
            //if (path.StartsWith("/Login/api/CheckUsernameAPI") || path.StartsWith("/Login/api/GetJwtAPI") || path.StartsWith("/AccCreationAPI/api/NormalAccCreationAPI"))
            if(_configuration.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>().Contains(path)
               /* || _configuration.GetSection("AllowedEndpoints:NormAd").Get<List<string>>().Contains(path)*/)
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
        var fullUrl = context.Request.GetDisplayUrl();

        // Parse the URL to get the path
        var uri = new Uri(fullUrl);
        var path = uri.AbsolutePath;


        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
        //Console.WriteLine(payloadJson);

        var jObject = JObject.Parse(payloadJson);

        string audience = jObject["aud"]!.ToString();
        string scope = jObject["scope"]!.ToString();
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


        //var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

        //// Parse the JSON to get the role or permissions
        //// This is a very basic way to parse JSON. Consider using a JSON library.
        //var permissions = new Regex("\"permissions\": \"([^\"]+)\"").Match(payloadJson).Groups[1].Value;

        //// Check if the permissions include what's needed
        //return permissions.Contains("requiredPermission");
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