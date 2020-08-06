using Hangfire.PostgreSql;
using Hangfire.Storage;
using System.Collections.Generic;
using Npgsql;

namespace Hangfire.MAMQSqlExtension
{
    public class MAMQPostgreSqlStorage : PostgreSqlStorage
    {
        private IEnumerable<string>? _queues;
        private PostgreSqlStorageOptions _postgreSqlStorageOptions;
        private string _nameOrConnectionString;

        public MAMQPostgreSqlStorage(string nameOrConnectionString, PostgreSqlStorageOptions options, IEnumerable<string>? queues) : base(nameOrConnectionString, options)
        {
            _queues = queues;
            _postgreSqlStorageOptions = options;
            _nameOrConnectionString = nameOrConnectionString;
        }

        public override IStorageConnection GetConnection()
        {
            return new MAMQPostgreSqlConnection(this, _queues, _postgreSqlStorageOptions, _nameOrConnectionString);
        }
    }
}