using Hangfire.Common;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Npgsql;

namespace Hangfire.MAMQSqlExtension
{
    internal class MAMQPostgreSqlConnection : JobStorageConnection
    {
        private readonly JobStorageConnection _postgreSqlConnection;
        private readonly IEnumerable<string>? _queues;

        public MAMQPostgreSqlConnection(PostgreSqlStorage storage, IEnumerable<string>? queues, PostgreSqlStorageOptions options, string nameOrConnectionString)
        {
            const string postgresSqlConnectionTypeName = "Hangfire.PostgreSql.PostgreSqlConnection";
            Type? type = typeof(PostgreSqlStorage)
                .Assembly
                .GetType(postgresSqlConnectionTypeName);

            if (type == null)
            {
                throw new InvalidOperationException($"{postgresSqlConnectionTypeName} has not been loaded into the process.");
            }
            var npgsqlConnection = new NpgsqlConnection(nameOrConnectionString);
            // NpgsqlConnection connection,
            // PersistentJobQueueProviderCollection queueProviders,
            //PostgreSqlStorageOptions options)

            var connection = type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Instance | BindingFlags.Public, null, new object[] { npgsqlConnection, storage.QueueProviders, options }, null, null) as JobStorageConnection;
            _postgreSqlConnection = connection ?? throw new InvalidOperationException($"Could not create an instance for {postgresSqlConnectionTypeName}");
            _queues = queues;
        }

        public override IWriteOnlyTransaction CreateWriteTransaction()
        {
            return _postgreSqlConnection.CreateWriteTransaction();
        }

        public override IDisposable AcquireDistributedLock(string resource, TimeSpan timeout)
        {
            return _postgreSqlConnection.AcquireDistributedLock(resource, timeout);
        }

        public override string CreateExpiredJob(Job job, IDictionary<string, string> parameters, DateTime createdAt, TimeSpan expireIn)
        {
            return _postgreSqlConnection.CreateExpiredJob(job, parameters, createdAt, expireIn);
        }

        public override IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken)
        {
            return _postgreSqlConnection.FetchNextJob(queues, cancellationToken);
        }

        public override void SetJobParameter(string id, string name, string value)
        {
            _postgreSqlConnection.SetJobParameter(id, name, value);
        }

        public override string GetJobParameter(string id, string name)
        {
            return _postgreSqlConnection.GetJobParameter(id, name);
        }

        public override JobData GetJobData(string jobId)
        {
            return _postgreSqlConnection.GetJobData(jobId);
        }

        public override StateData GetStateData(string jobId)
        {
            return _postgreSqlConnection.GetStateData(jobId);
        }

        public override void AnnounceServer(string serverId, ServerContext context)
        {
            _postgreSqlConnection.AnnounceServer(serverId, context);
        }

        public override void RemoveServer(string serverId)
        {
            _postgreSqlConnection.RemoveServer(serverId);
        }

        public override void Heartbeat(string serverId)
        {
            _postgreSqlConnection.Heartbeat(serverId);
        }

        public override int RemoveTimedOutServers(TimeSpan timeOut)
        {
            return _postgreSqlConnection.RemoveTimedOutServers(timeOut);
        }

        public override HashSet<string> GetAllItemsFromSet(string key)
        {
            return _postgreSqlConnection.GetAllItemsFromSet(key);
        }

        public override string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore)
        {
            if (!_queues.Any())
                return null;

            var id = _postgreSqlConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore);

            if (_queues == null)
                return id;

            var recurringJobId = key switch
            {
                "recurring-jobs" => id,
                "schedule" => _postgreSqlConnection.GetJobParameter(id, "RecurringJobId")?.Replace("\"", ""),
                _ => throw new InvalidOperationException($"{key} is not a recognized job type")
            };

            var queue = recurringJobId switch
            {
                null => "default",
                _ => _postgreSqlConnection.GetValueFromHash($"recurring-job:{recurringJobId}", "Queue") ?? "default"
            };

            return _queues.Contains(queue) ? id : null;

            // return id.Where(id =>
            //     {
            //         string? recurringJobId = key switch
            //         {
            //             "recurring-jobs" => id,
            //             "schedule" => _postgreSqlConnection.GetJobParameter(id, "RecurringJobId")?.Replace("\"", ""),
            //             _ => throw new InvalidOperationException($"{key} is not a recognized job type")
            //         };
            //
            //         string queue = recurringJobId switch
            //         {
            //             null => "default",
            //             _ => _postgreSqlConnection.GetValueFromHash($"recurring-job:{recurringJobId}", "Queue") ?? "default"
            //         };
            //
            //         return _queues.Contains(queue);
            //     }).SingleOrDefault<string>();
            //return _postgreSqlConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore);
        }

        public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            _postgreSqlConnection.SetRangeInHash(key, keyValuePairs);
        }

        public override Dictionary<string, string> GetAllEntriesFromHash(string key)
        {
            return _postgreSqlConnection.GetAllEntriesFromHash(key);
        }
    }
}