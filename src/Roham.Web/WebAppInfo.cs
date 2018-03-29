using System;
using System.Reflection;
using Roham.Lib.Ioc;
using Roham.Domain.Configs;

namespace Roham.Web
{
    public interface IApplicationInfo
    {
        string Name { get; }        
        string Version { get; }
        string Copyright { get; }
        bool IsConfigFileMissing { get; }
        bool IsInstalled { get; }
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class ApplicationInfo : IApplicationInfo
    {
        private readonly Func<IRohamConfigs> _configsResolver;
        private readonly AssemblyInfo _assemblyInfo;

        public ApplicationInfo(Func<IRohamConfigs> configsResolver)
        {
            _configsResolver = configsResolver;
            _assemblyInfo = new AssemblyInfo(typeof(ApplicationInfo).Assembly);
        }

        public string Name => Getname();        
        public string Version => _assemblyInfo.Version;
        public string Copyright => $"{_assemblyInfo.Copyright.Replace("{DATE}", DateTime.Now.Year.ToString())} {Name} {Version}";
        public bool IsConfigFileMissing => _configsResolver().IsConfigFileMissing;
        public bool IsInstalled => _configsResolver().IsInstalled;        

        private string Getname()
        {
            var configs = _configsResolver();
            if (configs.IsInstalled)
            {
                return configs.AppName;
            }
            return _assemblyInfo.Title;
        }
    }

    // A wrapper which is convenient to be used in views
    public static class WebAppInfo
    {
        private volatile static Lazy<IApplicationInfo> _lazyAppInfo;

        static WebAppInfo()
        {
            _lazyAppInfo = new Lazy<IApplicationInfo>(() => RohamDependencyResolver.Current.Resolve<IApplicationInfo>());
        }

        public static string Name => _lazyAppInfo.Value.Name;
        public static string Version => _lazyAppInfo.Value.Version;
        public static string Copyright => _lazyAppInfo.Value.Copyright;
        public static bool IsConfigFileMissing => _lazyAppInfo.Value.IsConfigFileMissing;
        public static bool IsInstalled => _lazyAppInfo.Value.IsInstalled;
    }
}
