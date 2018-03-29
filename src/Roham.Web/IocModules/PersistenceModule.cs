using AF = Autofac;
using Autofac;
using Roham.Data;
using Roham.Data.Providers;
using Roham.Lib.Domain.Persistence;
using Roham.Persistence.NHibernate;
using Roham.Domain.Entities.Security;

namespace Roham.Web.IocModules
{
    public class PersistenceModule : AF.Module
    {
        protected override void Load(AF.ContainerBuilder builder)
        {
            base.Load(builder);

            // Database Providers
            builder
                .RegisterType<DatabaseProviderFactory>()
                .As<IDatabaseProviderFactory>()
                .SingleInstance();

            builder
                .RegisterType<SqlServerDatabaseProvider>()
                .Named<IDatabaseProvider>(DbProviders.SqlServer.ToString());

            builder
                .RegisterType<SQLiteDatabaseProvider>()
                .Named<IDatabaseProvider>(DbProviders.SQLite.ToString());

            builder
                .Register(c =>
                {
                    var factory = c.Resolve<IDatabaseProviderFactory>();
                    var config = c.Resolve<IPersistenceConfigs>();
                    return factory.Create(config.DatabaseProvider);
                })
                .As<IDatabaseProvider>()
                .InstancePerLifetimeScope();

            builder
                .Register(c =>
                {
                    return new NHPersistenceContextFactory(
                        c.Resolve<IPersistenceConfigs>(),
                        c.Resolve<IDatabaseProviderFactory>(),
                        typeof(UserMapping).Assembly);
                })
                .As<IPersistenceContextFactory>()
                .As<IPersistenceMetaContextFactory>()
                .SingleInstance();

            builder
                .RegisterType<PersistenceUnitOfWorkFactory>()
                .AsSelf()
                .As<IPersistenceUnitOfWorkFactory>()
                .SingleInstance();
        }
    }
}