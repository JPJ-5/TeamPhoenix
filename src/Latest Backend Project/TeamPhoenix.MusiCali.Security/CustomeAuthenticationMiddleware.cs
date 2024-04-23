using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Microsoft.IdentityModel.Tokens;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;

    }

    public async Task Invoke(HttpContext context)
    {
        var idToken = context.Request.Headers["Authentication"].FirstOrDefault();

        if ((!idToken.IsNullOrEmpty()) && checkSignature(idToken!) && ValidateToken(idToken!))
        {
            await _next(context);
            return;
        }
        else
        {
            Console.WriteLine("Here");
            context.Response.StatusCode = 401; // Unauthenticated
            return;
        }

    }

    private bool checkSignature(string idToken)
    {
        List<string> jwtParts = getJWTParts(idToken);
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
    private bool ValidateToken(string idToken)
    {
        try
        {
            //Console.WriteLine(idToken);

            List<string> jwtParts = getJWTParts(idToken);
            var header = jwtParts[0];
            var payload = jwtParts[1];
            var signature = jwtParts[2];

            var decodedPayload = Base64UrlDecode(payload);
            //Console.WriteLine(payloadJson);

            using JsonDocument doc = JsonDocument.Parse(decodedPayload);
            JsonElement root = doc.RootElement;


            string audience = root.GetProperty("aud").GetString()!;
            //Console.WriteLine(audience);
            if (_configuration.GetSection("Jwt:Audience").Value!.Equals(audience))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
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