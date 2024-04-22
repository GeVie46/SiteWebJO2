using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TextTemplating;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using SiteWebJO2.Services;

namespace SiteWebJO2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            /*
             * connect to local database MySQL (MariaDB)
             * password stored in user secrets manager
             * Development server version: new MariaDbServerVersion(new Version(10, 4, 28))
             */
            //var connectionString = (builder.Configuration.GetConnectionString("MySqlBaseDevConnection") + ";pwd=" + builder.Configuration["DbPassword"]).ToString();
            //if (connectionString.IsNullOrEmpty())
            //{
            //    throw new InvalidOperationException("Connection string 'connection' not found.");
            //}
            //builder.Services.AddDbContext<ApplicationDbContext>(options => options

            //    .UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 28)))
            //    // The following three options help with debugging, but should
            //    // be changed or removed for production.
            //    .LogTo(Console.WriteLine, LogLevel.Information)
            //    .EnableSensitiveDataLogging()
            //    .EnableDetailedErrors()
            //);

            /*
             * connect to production database MySQL (MariaDB) on Always Data
             * DATABASE_URL : environment variable to be declared in the secret of the app on fly.io. connection string to the database
             */

            //to update production database
            //var MySqlBaseProdConnection = builder.Configuration["DATABASE_URL"].ToString();

            var MySqlBaseProdConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (MySqlBaseProdConnection == null)
            {
                throw new InvalidOperationException("Connection string 'MySqlBaseProdConnection' not found (check that environment variable DATABASE_URL exists on fly.io).");
            }
            builder.Services.AddDbContext<ApplicationDbContext>(options => options
                //version du serveur
                .UseMySql(MySqlBaseProdConnection, new MariaDbServerVersion(new Version(10, 6, 16)))
            );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                /*
                 * Configure app for use of Identity roles
                 */
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            /*
             * * Configure app for hot reload
             */
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            /*
             * Configure app to support email
             * code from 
             * https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-8.0&tabs=visual-studio
             */
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

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
