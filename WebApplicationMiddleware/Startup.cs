using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationMiddleware
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
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async(context, next) => 
            {
                await context.Response.WriteAsync("Middleware 1 in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Middleware 1 out. \r\n");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware 2 in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Middleware 2 out. \r\n");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware 3 in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Middleware 3 out. \r\n");
            });

            //加入自定義的Mddleware
            app.UseMiddleware<MyMiddleware>();

            //注意是用.Run是Middleware的終點，放在這個位置之後的Middleware4不會被執行到，注意執行順序
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World! \r\n");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Middleware 4 in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Middleware 4 out. \r\n");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
