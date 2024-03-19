using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Security.Contracts;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services required for MVC
        services.AddMvc();

        // Register Authentication and Authorization services
        services.AddSingleton<IAuthentication, Authentication>();
        services.AddSingleton<IAuthorization, Authorization>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Route for handling user search
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/search", async context =>
            {
                try
                {
                    var username = context.Request.Query["username"];

                    // Authenticate user
                    var authenticationService = context.RequestServices.GetRequiredService<IAuthentication>();
                    var tokens = authenticationService.Authenticate(username, ""); // Pass OTP as empty for now

                    // Authorize user
                    var authorizationService = context.RequestServices.GetRequiredService<IAuthorization>();
                    var userPrincipal = new Principal(username, new Dictionary<string, string>()); // Create Principal with username (roles can be added if needed)
                    if (!authorizationService.IsUserAuthorized(userPrincipal, "search", "read"))
                    {
                        context.Response.StatusCode = 403; // Forbidden
                        await context.Response.WriteAsync("Access Denied");
                        return;
                    }

                    // Log the search operation using the Logger class
                    var logger = new Logger();
                    logger.CreateLog("UserHash", "Info", "View", $"Search operation initiated. Username: {username}");

                    // Create MySQL connection
                    using (var connection = new MySqlConnection("Server=localhost;Database=your_database_name;Uid=your_username;Pwd=your_password;"))
                    {
                        await connection.OpenAsync();

                        // Construct SQL query based on search criteria
                        var sql = "SELECT * FROM users WHERE 1=1";
                        var parameters = new MySqlParameterCollection();
                        if (!string.IsNullOrEmpty(username))
                        {
                            sql += " AND username = @username";
                            parameters.AddWithValue("@username", username);
                        }

                        // Check user visibility setting
                        sql += " AND visibility = 1"; // Assuming 1 means visible
                        // Uncomment the line below if user visibility setting is stored in the database
                        // sql += " AND visibility = (SELECT visibility FROM users WHERE username = @username)";

                        using (var command = new MySqlCommand(sql, connection))
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                var dataTable = new DataTable();
                                dataTable.Load(reader);

                                // Convert DataTable to JSON and send response
                                var json = JsonSerializer.Serialize(dataTable);
                                await context.Response.WriteAsync(json);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error message using the Logger class
                    var logger = new Logger();
                    logger.CreateLog("UserHash", "Error", "Server", $"Error searching users: {ex.Message}");

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error");
                }
            });

            // Route for handling user visibility change
            endpoints.MapPost("/visibility", async context =>
            {
                try
                {
                    // Extract username and visibility setting from request
                    var username = context.Request.Form["username"];
                    var visibility = context.Request.Form["visibility"];

                    // Log visibility change
                    var logger = new Logger();
                    logger.CreateLog("UserHash", "Info", "View", $"User has {(visibility == "1" ? "enabled" : "disabled")} visibility");

                    // Update user visibility setting in the database
                    // Uncomment the code below and replace placeholders with actual database update logic
                    //using (var connection = new MySqlConnection("Server=localhost;Database=your_database_name;Uid=your_username;Pwd=your_password;"))
                    //{
                    //    await connection.OpenAsync();
                    //    var sql = "UPDATE users SET visibility = @visibility WHERE username = @username";
                    //    using (var command = new MySqlCommand(sql, connection))
                    //    {
                    //        command.Parameters.AddWithValue("@username", username);
                    //        command.Parameters.AddWithValue("@visibility", visibility);
                    //        await command.ExecuteNonQueryAsync();
                    //    }
                    //}

                    // Return success response
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Visibility updated successfully");
                }
                catch (Exception ex)
                {
                    // Log error message using the Logger class
                    var logger = new Logger();
                    logger.CreateLog("UserHash", "Error", "Server", $"Error updating user visibility: {ex.Message}");

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error");
                }
            });
        });
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
