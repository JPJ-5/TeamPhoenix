using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using Microsoft.EntityFrameworkCore;
using TeamPhoenix.MusiCali.Security;
using Authentication = TeamPhoenix.MusiCali.Security.Authentication;
using TeamPhoenix.MusiCali.Security.Contracts;

namespace AccCreationAPI
{
    public class Program
    {
        public IConfiguration Configuration { get; }

        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<LogoutRepository>();
            builder.Services.AddScoped<LogoutService>();
            builder.Services.AddScoped<IAuthentication, Authentication>();

            builder.Services.AddScoped<MariaDB>();          // Register MariaDB Class with Dependency Injection 
            //jwt token

            var tkConf = builder.Configuration.GetSection("Jwt");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tkConf["Issuer"],
                ValidAudience = tkConf["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tkConf["Key"]!))
            };

            // Configuring CORS using settings from appsettings.json
            var corsPolicy = builder.Configuration.GetSection("CorsPolicy");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CustomCorsPolicy", policy =>
                {
                    var allowedOrigins = corsPolicy.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                    var allowedMethods = corsPolicy.GetSection("AllowedMethods").Get<string[]>() ?? Array.Empty<string>();
                    var allowedHeaders = corsPolicy.GetSection("AllowedHeaders").Get<string[]>() ?? Array.Empty<string>();

                    policy.WithOrigins(allowedOrigins)
                          .WithMethods(allowedMethods)
                          .WithHeaders(allowedHeaders);

                    // Conditionally allow credentials based on configuration
                    if (corsPolicy.GetValue<bool>("AllowCredentials"))
                    {
                        policy.AllowCredentials();
                    }
                    else
                    {
                        policy.DisallowCredentials();
                    }
                });
            });

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

            //app.UseMiddleware<AuthenticationMiddleware>();

            // Then register the AuthorizationMiddleware
            //app.UseMiddleware<AuthorizationMiddleware>();


            app.MapControllers();

            app.Run();
        }

    }
}
