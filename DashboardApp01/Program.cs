using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DashboardApp01
{
    public class Program
    {
        private static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseResultsInContinuations()
                .UsePostgreSqlStorage(@"Host=localhost;Database=hangfire_test;Username=postgres;Password=innroad;Pooling=true;MinPoolSize=50;MaxPoolSize=1024;Connection Idle Lifetime=180;Timeout=30;", new PostgreSqlStorageOptions()
                {
                    PrepareSchemaIfNecessary = true
                    // CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    // QueuePollInterval = TimeSpan.Zero,
                    // SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1),
                    // UseRecommendedIsolationLevel = true,
                    // UsePageLocksOnDequeue = true,
                    // DisableGlobalLocks = true,
                    // EnableHeavyMigrations = true
                });

            CreateHostBuilder(args)
                .Build()
                .Run();

            // using (WebApp.Start<Startup>("http://localhost:12345"))
            // {
            //     Console.WriteLine("Press Enter to exit...");
            //     Console.ReadLine();
            // }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseUrls("http://0.0.0.0:12345");
                    builder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                });
    }
}