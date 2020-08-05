using ClassLibraryApp1;
using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.PostgreSql;
using System;

namespace ConsoleApp02
{
    internal class Program
    {
        public static void Main()
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseResultsInContinuations()
                .UseMAMQPostgreSQLStorage(@"Host=localhost;Database=hangfire_test;Username=postgres;Password=innroad;Pooling=true;MinPoolSize=50;MaxPoolSize=1024;Connection Idle Lifetime=180;Timeout=30;", new PostgreSqlStorageOptions
                {
                    // UsePageLocksOnDequeue = true,
                    // DisableGlobalLocks = true,
                }, new[] { "app1_queue" });

            RecurringJob.AddOrUpdate("app1_job", () => App1_Tasks.Do_App1_Task(), Cron.Minutely, TimeZoneInfo.Local, "app1_queue");

            var serverOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "app1_queue", "default" },
            };

            using (var server = new BackgroundJobServer(serverOptions))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}