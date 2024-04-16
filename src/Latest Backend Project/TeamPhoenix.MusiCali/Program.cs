//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
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

            // Use the custom Authentication Middleware
            app.UseMiddleware<AuthenticationMiddleware>();

            // Use the Authorization Middleware
            app.UseMiddleware<AuthorizationMiddleware>();

            app.MapControllers();

            app.Run();
        }

    }
}
