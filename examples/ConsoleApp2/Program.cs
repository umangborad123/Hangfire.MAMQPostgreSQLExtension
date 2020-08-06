using ClassLibraryApp2;
using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.PostgreSql;
using System;

namespace ConsoleApp2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseResultsInContinuations()
                .UseMAMQPostgreSQLStorage(
                    @"Host=localhost;Database=hangfire_test;Username=postgres;Password=innroad;Pooling=true",
                    new PostgreSqlStorageOptions
                    {
                        EnableTransactionScopeEnlistment = true,
                        PrepareSchemaIfNecessary = true
                        // UsePageLocksOnDequeue = true,
                        // DisableGlobalLocks = true,
                    }, new[] { "app2_queue" });

            RecurringJob.AddOrUpdate("app2_job", () => App2_Tasks.Do_App2_Task(), Cron.Minutely, TimeZoneInfo.Local, "app2_queue");

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "app2_queue" }
            };
            using var server = new BackgroundJobServer(options);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}