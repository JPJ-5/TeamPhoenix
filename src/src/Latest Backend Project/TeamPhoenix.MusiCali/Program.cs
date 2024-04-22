using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.Services;

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

            builder.Services.AddScoped<MariaDB>();          // Register MariaDB Class with Dependency Injection 
            //jwt token

            var tkConf = builder.Configuration.GetSection("Jwt");
//            if (tkConf == null || string.IsNullOrEmpty(tkConf["Issuer"]) || string.IsNullOrEmpty(tkConf["Audience"]) || string.IsNullOrEmpty(tkConf["Key"]))
//{
//                throw new InvalidOperationException("JWT configuration is missing or incomplete in appsettings.json.");
//            }
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = tokenValidationParameters;
                });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins",
                    builder => builder
                        .WithOrigins("http://localhost:8800", "https://themusicali.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("MyAllowSpecificOrigins",
            //        policyBuilder =>
            //        {
            //            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            //            policyBuilder.WithOrigins(allowedOrigins) // Dynamically set origins based on configuration
            //                .AllowAnyHeader()
            //                .AllowAnyMethod()
            //                .WithExposedHeaders("Custom-Header1", "Custom-Header2") // Specify exposed headers
            //                .SetPreflightMaxAge(TimeSpan.FromSeconds(600)) // Set preflight cache duration
            //                .AllowCredentials(); // Allow credentials
            //        });
            //});


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyAllowSpecificOrigins");

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }



        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    // Additional custom validation for claims
                    NameClaimType = "sub", // Setting subject claim type
                    RoleClaimType = "role" // You can set a role claim type if you have role-based authorization
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Additional validation based on 'nonce' or other claims can be done here
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers();
            // ... any other services you need to configure
        }
    }
}
