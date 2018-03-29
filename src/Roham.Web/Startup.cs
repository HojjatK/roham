using System;
using Owin;
using Microsoft.Owin;
using Roham.Lib.Logger;
using Roham.Domain.Configs;
using Roham.Domain.Services;

[assembly: OwinStartup(typeof(Roham.Web.Startup))]
namespace Roham.Web
{
    public partial class Startup
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<Startup>();

        public void Configuration(IAppBuilder app)
        {
            if (!LoggerFactory.Configure())
            {
                throw new ApplicationException("Logger cannot be initialized");
            }
            Log.Info("++========== Roham Application Startup =============");

            ConfigureRoutes();
            ConfigureBinders();
            ConfigureDependencyResolver();

            var rohamConfigs = RohamDependencyResolver.Current.Resolve<IRohamConfigs>();
            var cacheService = RohamDependencyResolver.Current.Resolve<ICacheService>();
            ConfigureOAuth(app, rohamConfigs);
            ConfigureMiddlewares(app, rohamConfigs, cacheService);

            Log.Info("Roham Application Configured Successfully");
        }
    }
}
