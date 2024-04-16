//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using Authentication = TeamPhoenix.MusiCali.Security.Authentication;
using TeamPhoenix.MusiCali.Security.Contracts;
using Microsoft.Extensions.Configuration;

namespace AccCreationAPI
{
    public class Program
    {
        private static IConfiguration? _configuration; // Static, nullable configuration

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Set _configuration from the builder directly, to avoid being null
            // Set _configuration from builder configuration to ensure it's never null in usage context
            _configuration = builder.Configuration ?? throw new InvalidOperationException("Configuration cannot be null");


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<LogoutRepository>();
            builder.Services.AddScoped<LogoutService>();
            builder.Services.AddScoped<IAuthentication, Authentication>();

            builder.Services.AddScoped<MariaDB>();          // Register MariaDB Class with Dependency Injection 
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use the custom CORS middleware
            app.UseCustomCors();
            //Console.WriteLine(path);


            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;

                // Use the null-forgiving operator (!) to assert _configuration is not null
                if (_configuration!.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>()!.Contains(path))
                {
                    await next(); // Skip the authentication and authorization middleware
                    return;
                } else {
                    app.UseMiddleware<AuthenticationMiddleware>();

                    // Use the Authorization Middleware
                    app.UseMiddleware<AuthorizationMiddleware>();
                }

            });


            app.MapControllers();

            app.Run();
        }


    }
}
