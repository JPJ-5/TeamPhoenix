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

                // Check if the path matches the anonymous endpoints
                if (_configuration!.GetSection("AllowedEndpoints:Anonymous").Get<List<string>>()!.Contains(path))
                {
                    await next(context); // Skip the authentication and authorization middleware
                    return;
                }
                else
                {
                    await next(context);
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

////using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using TeamPhoenix.MusiCali.DataAccessLayer;
//using TeamPhoenix.MusiCali.Services;
//using Authentication = TeamPhoenix.MusiCali.Security.Authentication;
//using TeamPhoenix.MusiCali.Security.Contracts;
//using Microsoft.Extensions.Configuration;

//namespace AccCreationAPI
//{
//    public class Program
//    {
//        private readonly IConfiguration _configuration;

//        public Program(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.

//            builder.Services.AddControllers();
//            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();
//            builder.Services.AddScoped<LogoutRepository>();
//            builder.Services.AddScoped<LogoutService>();
//            builder.Services.AddScoped<IAuthentication, Authentication>();

//            builder.Services.AddScoped<MariaDB>();          // Register MariaDB Class with Dependency Injection 
//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();

//            // Use the custom CORS middleware
//            app.UseCustomCors();
//            //Console.WriteLine(path);

//            var anonymousEndpoints = new List<string>
//            {
//                "/Login/api/CheckUsernameAPI",
//                "/Login/api/GetJwtAPI",
//                "/AccCreationAPI/api/NormalAccCreationAPI",
//                "/api/RecoverUser"
//            };

//            app.Use(async (context, next) =>
//            {
//                var path = context.Request.Path;

//                // Check if the path matches the anonymous endpoints
//                if (IsAnonymousEndpoint(path, anonymousEndpoints))
//                {
//                    await next(context); // Skip the authentication and authorization middleware
//                    return;
//                }
//                else
//                {
//                    await next(context);
//                    app.UseMiddleware<AuthenticationMiddleware>();
//                    app.UseMiddleware<AuthorizationMiddleware>();
//                }
//            });


//            app.MapControllers();

//            app.Run();
//        }

//        private static bool IsAnonymousEndpoint(PathString path, List<string> anonymousEndpoints)
//        {
//            foreach (var endpoint in anonymousEndpoints)
//            {
//                if (path.StartsWithSegments(endpoint))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }


//    }
//}