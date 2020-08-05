using ClassLibraryApp2;
using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.SqlServer;
using System;
using Hangfire.PostgreSql;

namespace ConsoleApp2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseMAMQPostgreSQLStorage(@"Host=localhost;Database=hangfire_test;Username=postgres;Password=innroad;Pooling=true;MinPoolSize=50;MaxPoolSize=1024;Connection Idle Lifetime=180;Timeout=30;", new PostgreSqlStorageOptions
                {
                    // UsePageLocksOnDequeue = true,
                    // DisableGlobalLocks = true,
                }, new[] { "app2_queue" })
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();

            RecurringJob.AddOrUpdate("app2_job", () => App2_Tasks.Do_App2_Task(), Cron.Minutely, TimeZoneInfo.Local, "app2_queue");

            var optoins = new BackgroundJobServerOptions
            {
                Queues = new[] { "app2_queue" }
            };
            using (var server = new BackgroundJobServer(optoins))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}