namespace UITest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure CORS to allow requests from your frontend domain
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMyLocalOrigin",
                    builder => builder.WithOrigins("https://themusicali.com:5000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // Allow credentials is important for requests that involve cookies or auth headers.
            });

            var app = builder.Build();

            // Use CORS policy
            app.UseCors("AllowMyLocalOrigin");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseDefaultFiles();         // added to make the index.html run as default file.
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}