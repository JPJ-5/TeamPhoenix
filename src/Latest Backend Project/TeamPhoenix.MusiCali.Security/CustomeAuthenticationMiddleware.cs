using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.Security.Contracts; // Ensure this is pointing to your interface
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features.Authentication; // Add this for GetRequiredService
using Authentication = TeamPhoenix.MusiCali.Security.Contracts;
using Mysqlx;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Resolve the authentication service from the scoped service provider
        var authenticationService = context.RequestServices.GetRequiredService<IAuthentication>();

        context.Request.EnableBuffering();
        var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        Console.WriteLine(bodyAsText);
        context.Request.Body.Position = 0;

        try
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsText);
            string? username = body?.username;
            string? otp = body?.otp;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(otp))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Username and OTP are required.");
                return;
            }
            if (body?.otp == null || body?.otp == "string") // If OTP is not provided, assume it's the first step of authentication
            {
                var verify = authenticationService.AuthenticateUsername(username); // This should also trigger OTP generation
                if (verify)
                {
                    //// Inform the client that the username is accepted and OTP is sent
                    //context.Response.StatusCode = StatusCodes.Status202Accepted;
                    //await context.Response.WriteAsync(verify.ToString());

                    // Set the content type to application/json
                    context.Response.ContentType = "application/json";

                    // Set the status code to 202 Accepted or any appropriate status
                    context.Response.StatusCode = StatusCodes.Status202Accepted;

                    // Create a JSON string representing a boolean value
                    string json = System.Text.Json.JsonSerializer.Serialize(new { success = true });

                    // Write the JSON string to the response
                    await context.Response.WriteAsync(json);

                    if (verify == true)
                    {
                        var tokens = authenticationService.Authenticate(username, otp);
                        Console.WriteLine(tokens);

                        if (tokens.Count > 0)
                        {
                            context.Items["IdToken"] = tokens["IdToken"];
                            context.Items["AccessToken"] = tokens["AccessToken"];
                            await _next(context);
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Invalid username or OTP.");
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid username.");
                    }
                    return; // Stop further processing to wait for the next request with OTP

                }
            }
        }
        catch (JsonReaderException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid request body.");
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An error occurred during authentication.");
        }
    }
}