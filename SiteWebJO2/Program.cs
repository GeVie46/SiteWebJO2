using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SiteWebJO2.Data;
using SiteWebJO2.Models;

namespace SiteWebJO2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //connect to local database MySQL (MariaDB)
            var connectionString = builder.Configuration.GetConnectionString("MySqlBaseDevConnection") ?? throw new InvalidOperationException("Connection string 'MySqlBaseDevConnectiontest' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options
                //version du serveur
                .UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 28)))
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
            );

            //connect to production database MySQL (MariaDB) on Always Data
            //var connectionString = builder.Configuration.GetConnectionString("MySqlBaseProdConnection") ?? throw new InvalidOperationException("Connection string 'MySqlBaseProdConnection' not found.");
            //builder.Services.AddDbContext<ApplicationDbContext>(options => options
            //    //version du serveur
            //    .UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 6, 16)))
            //);

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                   
                //add use of Identity roles in app
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            // GV : ajout du hot reload
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
