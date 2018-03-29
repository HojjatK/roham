using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Roham.Ioc.Autofac;
using IOC = Roham.Lib.Ioc;

namespace Roham.Web
{
    public partial class Startup
    {
        private void ConfigureDependencyResolver()
        {
            var settingsFilePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["SettingsFilePath"]);
            int cacheDurationInMinutes = int.Parse(ConfigurationManager.AppSettings["CacheDurationInMinutes"]);

            // build autofac container
            var builder = new ContainerBuilder();
            var autoRegistration = new IOC.AutoRegistration(AutofacIocFactory.CreateRegistrator(builder));
            autoRegistration
                        .IncludeAssembliesFromCurrentDomain(a => a.FullName.StartsWith("Roham"))
                        .IncludeImplementsITypeNameConvention()
                        .IncludeClosingTypeConvention()
                        .ApplyRegistrations();

            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly()).PropertiesAutowired();

            builder.RegisterModule(new IocModules.ConfigsModule(settingsFilePath));
            //builder.RegisterModule(new IocModules.CachingModule(cacheDurationInMinutes));
            builder.RegisterModule(new IocModules.PersistenceModule());
            builder.RegisterModule(new IocModules.IdentityModule());

            builder
                .RegisterType(AutofacIocFactory.GetLifetimeScopeType())
                .As<IOC.IResolver>()
                .As<IOC.ILifetimeScope>()
                .InstancePerLifetimeScope();

            var container = builder.Build();

            // set dependency resolver
            var rootScope = container.Resolve<IOC.ILifetimeScope>();
            RohamDependencyResolver.Initialize(rootScope);

            //webapi
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // mvc
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
