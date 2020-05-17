using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlData;
using SqlData.SqlGenerator;
using Microsoft.AspNetCore.Authorization;
using FastReport.Data;
using System.Globalization;

namespace CentreApp
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
            ConfigStatic.XanNav = Configuration["XanNav"];
            ConfigStatic.XanIndex = Configuration["XanIndex"];
            ConfigStatic.XanLogin = Configuration["XanLogin"];
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                   options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
               });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
            Cs.CsStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddTransient<ISqlData, SqlGenerator>();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjMzODU0QDMxMzgyZTMxMmUzMGplN2QrWTJJb0NDd1ppRFl1dXNaaTlIdXdtOU1VSnpzOGJaUUd1UUtBMHM9");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //var cultureInfo = new CultureInfo("en-US");
            //CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
                }
            });

            app.UseCookiePolicy();


            app.UseAuthentication();
            app.UseFastReport();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
