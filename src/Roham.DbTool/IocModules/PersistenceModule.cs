using Autofac;
using AF = Autofac;
using Roham.Persistence.NHibernate;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Security;
using Roham.Data;

namespace Roham.DbTool.IocModules
{
    public class PersistenceModule : AF.Module
    {
        protected override void Load(AF.ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterType<DatabaseProviderFactory>()
                .As<IDatabaseProviderFactory>()
                .SingleInstance();

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