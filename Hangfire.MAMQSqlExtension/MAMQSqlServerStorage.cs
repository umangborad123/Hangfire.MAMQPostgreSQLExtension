namespace Hangfire.MAMQSqlExtension
{
    using Hangfire.SqlServer;
    using Hangfire.Storage;
    using System.Collections.Generic;

    public class MAMQSqlServerStorage : SqlServerStorage
    {
        private IEnumerable<string>? _queues;

        public MAMQSqlServerStorage(string nameOrConnectionString, SqlServerStorageOptions options, IEnumerable<string>? queues) : base(nameOrConnectionString, options)
        {
            _queues = queues;
        }

        public override IStorageConnection GetConnection()
        {
            return new MAMQSqlServerConnection(this, _queues);
        }
    }
}