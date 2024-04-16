using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

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
        var idToken = context.Request.Headers["Authentication"].FirstOrDefault()?.Split(" ").Last();


        if (idToken == null)
        {
            var path = context.Request.Path;
            //Console.WriteLine(path);

            // Check if the path matches your criteria
            if(_configuration.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>().Contains(path))
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

        if (ValidateToken(idToken))
        {
            await _next(context);
            return;
        }
        else{
            Console.WriteLine("Here");
            context.Response.StatusCode = 401; // Unauthenticated
            return;
        }

    }

    private bool ValidateToken(string idToken)
    {
        try
        {
            //Console.WriteLine(idToken);
            var parts = idToken.Split('.');
            if (parts.Length != 3)
                return false;

            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var computedSignature = SignatureComputeHmacSha256(header, payload);
            //Console.WriteLine(computedSignature);

            var decodedPayload = Base64UrlDecode(payload);
            //Console.WriteLine(payloadJson);

            using JsonDocument doc = JsonDocument.Parse(decodedPayload);
            JsonElement root = doc.RootElement;


            string audience = root.GetProperty("aud").GetString()!;
            //Console.WriteLine(audience);

            Console.WriteLine(signature);
            Console.WriteLine(computedSignature);
            

            if (signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase) 
                && _configuration.GetSection("Jwt:Audience").Value!.Equals(audience))
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