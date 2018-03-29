using System.Reflection;
using FluentNHibernate.Cfg.Db;
using Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate.Configurators
{
    internal abstract class PersistenceProviderBase : INHPersistenceProviderConfigurator
    {
        protected readonly IPersistenceConfigs _configs;

        public PersistenceProviderBase(Assembly mappingsAssembly, IPersistenceConfigs configs)
        {   
            MappingsAssembly = mappingsAssembly;
            _configs = configs;
        }

        public Assembly MappingsAssembly { get; }

        public void UpdateSchema(string connectionString)
        {
            NHConfigurator.SchemaUpdate(GetProviderConfigurer(connectionString), MappingsAssembly);
        }

        public void ExportSchemaScript(string connectionString, string filePath)
        {
            NHConfigurator.SchemaScriptExport(GetProviderConfigurer(connectionString), MappingsAssembly, filePath, false);
        }

        IPersistenceConfigurer INHPersistenceProviderConfigurator.GetConfigurer(string connectionString)
        {
            return GetProviderConfigurer(connectionString);
        }

        CfgSessionFactory INHPersistenceProviderConfigurator.CreateSessionFactory(string connectionString)
        {
            return NHConfigurator.CreateSessionFactory(GetProviderConfigurer(connectionString), MappingsAssembly,
                cfg =>
                {
                    cfg.SetProperty(global::NHibernate.Cfg.Environment.CurrentSessionContextClass, "thread_static"); // TODO: read from config
                    cfg.SetProperty(global::NHibernate.Cfg.Environment.ReleaseConnections, "on_close"); // TODO: read from config
                });
        }

        protected abstract IPersistenceConfigurer GetProviderConfigurer(string connectionString);
    }
}
