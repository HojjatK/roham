using Autofac;
using AF = Autofac;
using Roham.Lib.Domain.Persistence;
using Roham.Data;

namespace Roham.DbTool.IocModules
{
    internal class ConfigModule : AF.Module
    {
        public ConfigModule(DbProviders providerName, bool showSql)
        {
            this.ProviderName = providerName;
            this.ShowSql = showSql;
        }

        private DbProviders ProviderName { get; set; }
        private bool ShowSql { get; set; }

        protected override void Load(AF.ContainerBuilder builder)
        {
            var dbUpConfigs = new DbToolPersistenceConfigs() { ShowSql = ShowSql };
            (dbUpConfigs as IDbToolConfigsUpdater).SetDatabase(ProviderName, "");
            builder
                .RegisterInstance(dbUpConfigs)
                .As<IPersistenceConfigs>()
                .SingleInstance();
        }
    }
}
