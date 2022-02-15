using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HealthCheck
{


    public class Startup
    {
        /// <summary>
        /// THe name of the folder that hosts (находится) the Angular app
        /// </summary>
        public static readonly string ClientApp = "ClientApp";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                //configuration.RootPath = "ClientApp/dist";
                //configuration.RootPath = "HealthCheck/dist";
                configuration.RootPath = string.Format("{0}/dist", ClientApp);
            });

            services.AddHealthChecks();

            //services.AddHealthChecks().AddCheck<ICMPHealthCheck>("ICMP");
            services.AddHealthChecks()
                .AddCheck("ICMP_01",
                          new ICMPHealthCheck("www.ryadel.com",
                          100))
                .AddCheck("ICMP_02",
                          new ICMPHealthCheck("www.google.com",
                          100))
                .AddCheck("ICMP_03",
                          new ICMPHealthCheck("1fdef1.11.com",
                          100));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline (middleware).
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            //first middleware (HTTP-to-HTTPs)
            app.UseHttpsRedirection();


            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    //Disable caching for all static files on 1 hour.
                    //context.Context.Response.Headers["Cache-Control"] = "max-age=3600";
                    context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
                }
            });

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            //add route matching to the the middleware pipeline
            // This middleware looks at the set of endpoints
            //defined in the app and selects the best match based on each incoming HTTP request
            app.UseRouting();

            //сопоставляет запрашиваем адрес с контроллером action в который может передаться id
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new CustomHealthCheckOptions());
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                //root folder spa app
                //spa.Options.SourcePath = "ClientApp";
                spa.Options.SourcePath = ClientApp;

                //все входящие запросы адресовывать ангуляр серверу 
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
