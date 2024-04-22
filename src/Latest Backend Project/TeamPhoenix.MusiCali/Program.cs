using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;
using AuthenticationSecurity = TeamPhoenix.MusiCali.Security.AuthenticationSecurity;
using TeamPhoenix.MusiCali.Security.Contracts;

namespace AccCreationAPI
{
    public class Program
    {
        private static IConfiguration? _configuration;

        public Program(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _configuration = builder.Configuration ?? throw new InvalidOperationException("Configuration cannot be null");

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<LogOutDAO>();
            builder.Services.AddScoped<LogoutService>();
            builder.Services.AddScoped<IAuthentication, AuthenticationSecurity>();

            builder.Services.AddScoped<MariaDBDAO>();          // Register MariaDB Class with Dependency Injection 

            // Add configuration
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration configuration = configurationBuilder.Build();

            builder.Services.AddTransient<DataAccessLayer>(); // Assuming a parameterless constructor or adjust accordingly
            builder.Services.AddTransient<ItemService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
           //}

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