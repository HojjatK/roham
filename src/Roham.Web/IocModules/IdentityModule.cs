using AF = Autofac;
using Autofac;
using Roham.Domain.Identity;
using Microsoft.AspNet.Identity;

namespace Roham.Web.IocModules
{
    public class IdentityModule : AF.Module
    {
        protected override void Load(AF.ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterType<UserStore>()
                .As<IUserStore<ApplicationUser, long>>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<RoleStore>()
                .As<IRoleStore<ApplicationRole, long>>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EmailService>()
                .AsSelf()
                .InstancePerDependency();

            builder
                .RegisterType<SmsService>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}