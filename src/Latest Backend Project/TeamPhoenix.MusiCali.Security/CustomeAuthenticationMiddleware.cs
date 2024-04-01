using Microsoft.AspNetCore.Http;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

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
        //Console.WriteLine(idToken);

        if (idToken == null)
        {
            var fullUrl = context.Request.GetDisplayUrl();

            // Parse the URL to get the path
            var uri = new Uri(fullUrl);
            var path = uri.AbsolutePath;
            Console.WriteLine(path);
            //Console.WriteLine(path);

            // Check if the path matches your criteria
            //if (path.StartsWith("/Login/api/CheckUsernameAPI") || path.StartsWith("/Login/api/GetJwtAPI") || path.StartsWith("/AccCreationAPI/api/NormalAccCreationAPI"))
            if(_configuration.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>().Contains(path)
                /*|| _configuration.GetSection("AllowedEndpoints:NormAd").Get<List<string>>().Contains(path)*/)
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
            




            var computedSignature = ComputeHmacSha256(header + "." + payload, "simple-key");
            //Console.WriteLine(computedSignature);

            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            //Console.WriteLine(payloadJson);

            var jObject = JObject.Parse(payloadJson);

            string audience = jObject["aud"]!.ToString();
            //Console.WriteLine(audience);

            if(signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase) 
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