using System;
using System.IO;
using System.Reflection;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Roham.Persistence.NHibernate.Conventions;

namespace Roham.Persistence.NHibernate
{
    internal class NHConfigurator
    {
        public static CfgSessionFactory CreateSessionFactory(
            IPersistenceConfigurer configurer, 
            Assembly mappingsAssembly, 
            Action<Configuration> cfgCustomActions = null)
        {
            return BuildSessionFactory(configurer, mappingsAssembly, cfgCustomActions ?? (cfg => { }));
        }

        public static void SchemaScriptExport(
            IPersistenceConfigurer configurer, 
            Assembly mappingsAssembly, 
            string filePath, 
            bool doUpdate)
        {
            BuildSessionFactory(configurer, mappingsAssembly, cfg => SaveSchemaScript(cfg, filePath, doUpdate));
        }

        public static void SchemaUpdate(
            IPersistenceConfigurer configurer, 
            Assembly mappingsAssembly)
        {
            BuildSessionFactory(configurer, mappingsAssembly, cfg => UpdateSchema(cfg));
        }

        private static CfgSessionFactory BuildSessionFactory(
            IPersistenceConfigurer configurer, 
            Assembly mappingAssembly, 
            Action<Configuration> cfgActions)
        {
            Configuration exposedCfg = null;
            var sessionFactory = Fluently
                .Configure()
                .Database(configurer)
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssembly(mappingAssembly)
                        .Conventions.AddFromAssemblyOf<PropertyConvention>();
                })
                .ExposeConfiguration(cfg =>
                {
                    cfgActions(cfg);
                    exposedCfg = cfg;
                })
                .ProxyFactoryFactory(typeof(DefaultProxyFactoryFactory))
                .BuildSessionFactory();

            return new CfgSessionFactory(exposedCfg, sessionFactory);
        }

        private static void SaveSchemaScript(Configuration cfg, string filePath, bool doUpdate)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            using (var sw = new StreamWriter(stream))
            {
                new SchemaExport(cfg).Execute(str => sw.Write(str), true, false);
            }
        }

        private static void UpdateSchema(Configuration cfg)
        {
            new SchemaUpdate(cfg).Execute(false, true);
        }
    }
}