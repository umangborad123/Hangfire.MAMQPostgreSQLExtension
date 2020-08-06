using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DashboardApp01
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(@"Host=localhost;Database=hangfire_test;Username=postgres;Password=innroad;Pooling=true;MinPoolSize=50;MaxPoolSize=1024;Connection Idle Lifetime=180;Timeout=30;");
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