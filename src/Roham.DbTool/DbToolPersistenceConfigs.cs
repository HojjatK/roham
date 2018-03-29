using Roham.Data;
using Roham.Lib.Domain.Persistence;

namespace Roham.DbTool
{
    public interface IDbToolConfigsUpdater
    {
        void SetDatabase(DbProviders dbProvider, string connectionString);
    }

    public class DbToolPersistenceConfigs : IPersistenceConfigs, IDbToolConfigsUpdater
    {
        public DbProviders DatabaseProvider { get; private set; }
        public string ConnectionString { get; private set; }
        public bool ShowSql { get; set; }
        public int AdoNetBatchSize => 200;

        void IDbToolConfigsUpdater.SetDatabase(DbProviders dbProvider, string connectionString)
        {
            DatabaseProvider = dbProvider;
            ConnectionString = connectionString;
        }
    }
}