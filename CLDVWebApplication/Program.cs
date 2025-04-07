using Microsoft.EntityFrameworkCore;
using CLDVWebApplication.Models;

namespace CLDVWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Register DbContext with SQL Server
            builder.Services.AddDbContext<EventEaseDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EventEaseDB")));

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Ensure static files (CSS, JS) are served
            app.UseRouting();
            app.UseAuthorization();

            // Ensure proper controller routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "booking",
                pattern: "Booking/{action=Index}/{id?}",
                defaults: new { controller = "Booking", action = "Index" });

            app.MapControllerRoute(
                name: "event",
                pattern: "Event/{action=Index}/{id?}",
                defaults: new { controller = "Event", action = "Index" });


            app.Run();
        }
    }
}
    