using ClassLibraryApp2;
using Hangfire;
using Hangfire.Logging;
using Hangfire.MAMQSqlExtension;
using Hangfire.SqlServer;
using System;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseMAMQSqlServerStorage(@"Server=.\SQLEXPRESS01;Database=hangfire_test;Trusted_Connection=True;", new SqlServerStorageOptions
                {
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
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