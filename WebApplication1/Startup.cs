using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Models;
using ShoppingCart.IOC;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultUI()
             .AddDefaultTokenProviders();



            services.AddControllersWithViews();
            services.AddRazorPages();

            //restrict the no. of times the user is allowed to guess his/her password



            services.Configure<IdentityOptions>(
                 options =>
                 {
                     options.Password.RequiredUniqueChars = 2;
                     options.Password.RequiredLength = 8;
                     options.Lockout.MaxFailedAccessAttempts = 5;
                 }
                );

            DependencyContainer.RegisterServices(services, Configuration.GetConnectionString("DefaultConnection"));

           services.AddAuthentication()
                  .AddGoogle(options =>
                  {
                      IConfigurationSection googleAuthNSection =
                          Configuration.GetSection("Authentication:Google");

                      options.ClientId = googleAuthNSection["ClientId"];
                      options.ClientSecret = googleAuthNSection["ClientSecret"];
                  });
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("logs/mylog-{Date}.txt");

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //}
            //else //production mode
            //{
            //    app.UseExceptionHandler("/Home/Error"); //any unhandled exceptions

            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var cacheMaxAge = "604800";
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //            Path.Combine(env.ContentRootPath, "ValuableFiles")),
            //    RequestPath = "/ValuableFiles",
            //    OnPrepareResponse = ctx => {
            //        ctx.Context.Response.Headers.Append(
            //                 "Cache-Control", $"public, max-age={cacheMaxAge}");
            //    }
            //});



            app.UseRouting();

            app.UseAuthentication(); //identifying the user
            app.UseAuthorization(); //granting privileges to different users

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
