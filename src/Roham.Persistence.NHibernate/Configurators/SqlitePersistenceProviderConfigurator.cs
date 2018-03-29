using System.Reflection;
using FluentNHibernate.Cfg.Db;
using Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate.Configurators
{
    internal class SqlitePersistenceProviderConfigurator : PersistenceProviderBase
    {
        public SqlitePersistenceProviderConfigurator(Assembly mappingsAssembly, IPersistenceConfigs configs) : base(mappingsAssembly, configs) { }

        protected override IPersistenceConfigurer GetProviderConfigurer(string connectionString)
        {
            // Sqlite database
            var c = SQLiteConfiguration
                .Standard
                .ConnectionString(connectionString)
                .DoNot;

            if (_configs.ShowSql)
                c.ShowSql();
            return c;
        }
    }
}