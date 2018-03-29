using System;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using FluentNHibernate.Cfg.Db;
using Roham.Data;
using Roham.Lib.Domain.Persistence;
using Roham.Persistence.NHibernate.Configurators;
using RohamModel = Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate
{
    internal interface INHPersistenceProviderConfigurator : IPersistenceMetaContext
    {
        IPersistenceConfigurer GetConfigurer(string connectionString);
        CfgSessionFactory CreateSessionFactory(string connectionString);
    }

    public class NHPersistenceContextFactory : RohamModel.IPersistenceContextFactory, RohamModel.IPersistenceMetaContextFactory
    {
        private readonly static object _sessionFactoryLock = new object();
        private static CfgSessionFactory _cfgSessionFactory;
        private readonly IPersistenceConfigs _persistenceConfiguration;
        private readonly Func<IDatabaseProvider> _databaseProvider;
        private readonly Assembly _mappingsAssembly;

        public NHPersistenceContextFactory(
            IPersistenceConfigs persistenceConfiguration,
            IDatabaseProviderFactory dbProviderFactory,
            Assembly mappingsAssembly)
        {
            Objects.Requires(persistenceConfiguration != null, () => new NullReferenceException(nameof(IPersistenceConfigs)));
            Objects.Requires(dbProviderFactory != null, () => new NullReferenceException(nameof(IDatabaseProviderFactory)));
            Objects.Requires(mappingsAssembly != null, () => new NullReferenceException(nameof(Assembly)));

            _persistenceConfiguration = persistenceConfiguration;
            _databaseProvider = () => dbProviderFactory.Create(persistenceConfiguration.DatabaseProvider);
            _mappingsAssembly = mappingsAssembly;
        }

        private CfgSessionFactory ConfiguredSessionFactory
        {
            get
            {
                if (_cfgSessionFactory == null)
                {
                    lock (_sessionFactoryLock)
                    {
                        if (_cfgSessionFactory == null)
                        {
                            _cfgSessionFactory = CreateConfigurator().CreateSessionFactory(_persistenceConfiguration.ConnectionString);                            
                        }
                    }
                }
                return _cfgSessionFactory;
            }
        }

        public ISessionFactory SessionFactory => ConfiguredSessionFactory.SessionFactory;
        private Configuration Configuration => ConfiguredSessionFactory.Configuration;

        public RohamModel.IPersistenceContext Create()
        {
            var session = SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Auto;            
            CurrentSessionContext.Bind(session);

            return new NHPersistenceContext(SessionFactory, _databaseProvider());
        }

        public IPersistenceMetaContext CreateMeta()
        {
            switch (_persistenceConfiguration.DatabaseProvider)
            {
                case DbProviders.SqlServer:
                    return new SqlPersistenceProviderConfigurator(_mappingsAssembly, _persistenceConfiguration);
                case DbProviders.SQLite:
                    return new SqlitePersistenceProviderConfigurator(_mappingsAssembly, _persistenceConfiguration);
                default:
                    throw new NotSupportedException($"{_persistenceConfiguration.DatabaseProvider} provider is not supported");
            }
        }

        internal INHPersistenceProviderConfigurator CreateConfigurator()
        {
            return CreateMeta() as INHPersistenceProviderConfigurator;
        }
    }
}