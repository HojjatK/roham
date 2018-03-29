using Roham.Data;
using Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate
{
    internal class NHFixturePersistenceConfiguration : IPersistenceConfigs//, IChangePersistenceConfigs
    {
        public DbProviders DatabaseProvider
        {
            get { return DbProviders.SQLite; }
        }

        public string ConnectionString
        {
            get;
            private set;
        }

        public int AdoNetBatchSize
        {
            get;
            set;
        }

        public bool ShowSql
        {
            get;
            set;
        }

        //void IChangePersistenceConfigs.Save(string dbProvierName, string connectionString)
        //{
        //    if (dbProvierName != DatabaseProviderNames.Sqlite)
        //    {
        //        throw new ArgumentException("dbProviderName");
        //    }
        //    ConnectionString = connectionString;
        //}
    }
}
