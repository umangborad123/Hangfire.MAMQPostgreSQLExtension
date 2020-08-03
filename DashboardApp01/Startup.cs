using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DashboardApp01
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(@"Server=.\SQLEXPRESS01;Database=hangfire_test;Trusted_Connection=True;");
            });
        }

        public void Configuration(IApplicationBuilder app)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                IgnoreAntiforgeryToken = true
            });
        }
    }
}