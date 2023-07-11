using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileServerApp
{
    public class Startup
    {
        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable routing
            app.UseRouting();

            // Configure endpoints
            app.UseEndpoints (endpoints => {
                // File server
                endpoints.MapGet("/fs", Handlers.FileServer);
                // Assets delivery
                endpoints.MapGet("/assets", Handlers.AssetsDelivery);
            });

            app.Run (Handlers.DefaultGet);
        }
    }
}
