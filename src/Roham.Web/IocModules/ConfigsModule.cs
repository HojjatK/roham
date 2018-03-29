using Autofac;
using AF = Autofac;
using Roham.Lib.Settings;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Configs;

namespace Roham.Web.IocModules
{
    public class ConfigsModule : AF.Module
    {
        private readonly string _settingsFilePath;
        public ConfigsModule(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        protected override void Load(AF.ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterInstance(new RohamConfigs(new XmlFileSettings(_settingsFilePath)))
                .As<IPersistenceConfigs>()
                .As<IRohamConfigs>()
                .SingleInstance();
        }
    }
}