using System.Data;
using System.Reflection;
using NHibernate.Connection;
using FluentNHibernate.Cfg.Db;
using Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate.Configurators
{
    internal class SqliteMemoryPersistenceProviderConfigurator : PersistenceProviderBase
    {
        public SqliteMemoryPersistenceProviderConfigurator(Assembly mappingsAssembly, IPersistenceConfigs configs) : base(mappingsAssembly, configs) { }

        protected override IPersistenceConfigurer GetProviderConfigurer(string connectionString)
        {
            // Sqlite in-memory database
            var c = SQLiteConfiguration
                .Standard
                .InMemory()
                .Provider<SqliteInMemoryConnectionProvider>();

            if (_configs.ShowSql)
                c.ShowSql();
            return c;
        }

        public void CloseDatabase()
        {
            SqliteInMemoryConnectionProvider.CloseDatabase();
        }

        #region Nested Classes

        private class SqliteInMemoryConnectionProvider : DriverConnectionProvider
        {
            private static object _connLock = new object();
            private volatile static IDbConnection _sharedConnection = null;

            public override IDbConnection GetConnection()
            {
                if (_sharedConnection == null)
                {
                    lock (_connLock)
                    {
                        if (_sharedConnection == null)
                            _sharedConnection = base.GetConnection();
                    }
                }

                return _sharedConnection;
            }

            public override void CloseConnection(IDbConnection conn)
            {
            }

            public static void CloseDatabase()
            {
                if (_sharedConnection != null)
                {
                    try
                    {
                        _sharedConnection.Close();
                    }
                    finally
                    {
                        _sharedConnection.Dispose();
                        _sharedConnection = null;
                    }
                }
            }
        }

        #endregion
    }
}
