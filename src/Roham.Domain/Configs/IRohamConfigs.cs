using Roham.Data;
using Roham.Lib.Domain.Persistence;

namespace Roham.Domain.Configs
{
    public interface IRohamConfigs : IPersistenceConfigs
    {        
        bool IsInstalled { get; }
        bool IsConfigFileMissing { get; }
        string InstallationPassword { get; }
        string AppName { get; }

        CacheProviders CacheProvider { get; }
        string CacheConnectionString { get; }

        bool SmtpEnabled { get; }
        string SmtpHost { get; }
        int SmtpPort { get; }
        bool SmtpEnableSsl { get; }
        string SmtpUsername { get; }
        string SmtpPassword { get; }
        string SmtpDomain { get; }
        string SmtpFrom { get; }
    }

    public interface IRohamConfigsUpdater
    {
        void SetInstall(bool installed);
        void SetAppName(string name);
        void SetDatabase(string dbProvider, string connectionString);
        void SetCacheProvider(string cacheProvider, string connectionString);
        void SetSmtp(bool enabled, string host, int port, bool enableSsl, string username, string password, string domain, string smtpFrom);
    }
}
