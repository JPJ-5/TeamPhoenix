using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;

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
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault();


        if ((!accessToken.IsNullOrEmpty()) && checkSignature(accessToken!) && HasRequiredPermissions(accessToken!, context))
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

    // Checking the token signature should be a function on its own

    private List<string> getJWTParts(string idToken)
    {
        var parts = idToken.Split('.');
        List<string> jwtParts = new List<string>();
        if (parts.Length != 3)
            return new List<string>();
        for (int i = 0; i < parts.Length; i++)
        {
            jwtParts.Insert(i, parts[i]);
        }
        return jwtParts;
    }
    private bool checkSignature(string accessToken)
    {
        List<string> jwtParts = getJWTParts(accessToken);
        if (jwtParts == new List<string>())
        {
            return false;
        }
        else
        {
            var header = jwtParts[0];
            var payload = jwtParts[1];
            var signature = jwtParts[2];

            var computedSignature = SignatureComputeHmacSha256(header, payload);
            //Console.WriteLine(computedSignature);
            if (signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
    private bool HasRequiredPermissions(string accessToken, HttpContext context)
    {
        List<string> jwtParts = getJWTParts(accessToken);
        var header = jwtParts[0];
        var payload = jwtParts[1];
        var signature = jwtParts[2];
        var computedSignature = SignatureComputeHmacSha256(header, payload);

        var path = context.Request.Path;


        var decodedPayload = Base64UrlDecode(payload);


        using JsonDocument doc = JsonDocument.Parse(decodedPayload);
        JsonElement root = doc.RootElement;

        string audience = root.GetProperty("aud").GetString()!;
        string scope = root.GetProperty("scope").GetString()!;
        //Console.WriteLine(scope);


        if (_configuration.GetSection("Jwt:Audience").Value!.Equals(audience)
            && scope == "NormalUser"
            && _configuration.GetSection("AllowedEndpoints:Normal").Get<List<string>>().Contains(path)
           )
        {
            return true;
        }
        if (_configuration.GetSection("Jwt:Audience").Value!.Equals(audience)
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

    private string SignatureComputeHmacSha256(string encodedHeader, string encodedPayload)
    {
        using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes("simple-key")))
        {
            // String to Byte[]
            var signatureInput = $"{encodedHeader}.{encodedPayload}";
            var signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

            // Byte[] to String
            var signatureDigestBytes = hash.ComputeHash(signatureInputBytes);
            var encodedSignature = WebEncoders.Base64UrlEncode(signatureDigestBytes);
            return encodedSignature;
        }
    }

    private static string Base64UrlDecode(string input)
    {

        var bytes = WebEncoders.Base64UrlDecode(input);

        return Encoding.UTF8.GetString(bytes);
    }
}