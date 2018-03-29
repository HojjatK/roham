using Roham.Data;
using Roham.Lib.Settings;
using System;
using System.Collections.Generic;

namespace Roham.Domain.Configs
{
    public class RohamConfigs : IRohamConfigs, IRohamConfigsUpdater
    {
        // Settings stored in roham config file
        private const string AppInstalledKey = "app.installed";
        private const string AppInstallationPasswordKey = "app.installation.password";
        private const string AppNameKey = "app.name";
        private const string DatabaseProviderKey = "database.provider";
        private const string ConnectionStringKey = "database.connection";
        private const string ShowSqlKey = "database.showsql";
        private const string CacheProviderKey = "cache.provider";
        private const string CacheConnectionStringKey = "cache.connection";
        private const string SmtpEnabledKey = "smtp.enabled";
        private const string SmtpHostKey = "smtp.host";
        private const string SmtpPortKey = "smtp.port";
        private const string SmtpEnableSslKey = "smtp.ssl";
        private const string SmtpUserNameKey = "smtp.username";
        private const string SmtpPasswordKey = "smtp.password";
        private const string SmtpDomainKey = "smtp.domain";
        private const string SmtpFromKey = "smtp.from";

        // Settings stored in  app configs        
        private const string ADONetBatchSizeKey = "ADONetBatchSize";
        private const string CacheDurationInMinutesKey = "CacheDurationInMinutes";


        private readonly XmlFileSettings _xmlFileSettings;

        public RohamConfigs(XmlFileSettings xmlFileSettings)
        {
            Objects.Requires(xmlFileSettings != null, () => new NullReferenceException(nameof(XmlFileSettings)));
            _xmlFileSettings = xmlFileSettings;
        }

        public bool IsInstalled
        {
            get
            {
                var installed = _xmlFileSettings.Get(AppInstalledKey);
                if (!string.IsNullOrWhiteSpace(installed))
                {
                    return bool.Parse(installed);
                }
                return false;
            }
        }

        public string AppName
        {
            get
            {
                if (IsInstalled)
                {
                    return _xmlFileSettings.Get(AppNameKey);
                }
                return "";
            }
        }

        public bool IsConfigFileMissing => _xmlFileSettings.IsSettingsFileMissing();

        public string InstallationPassword => _xmlFileSettings.Get(AppInstallationPasswordKey);

        public DbProviders DatabaseProvider
        {
            get
            {
                var result = DbProviders.None;
                var dbProviderStr = _xmlFileSettings.Get(DatabaseProviderKey);
                if (!string.IsNullOrWhiteSpace(dbProviderStr))
                {
                    if (Enum.TryParse(dbProviderStr, out result))
                    {
                        return result;
                    }
                }
                return result;
            }
        }

        public string ConnectionString => _xmlFileSettings.Get(ConnectionStringKey);

        public CacheProviders CacheProvider
        {
            get
            {
                var result = CacheProviders.Memory;
                var cacheProviderStr = _xmlFileSettings.Get(CacheProviderKey);
                if (!string.IsNullOrWhiteSpace(cacheProviderStr))
                {
                    if (Enum.TryParse(cacheProviderStr, out result))
                    {
                        return result;
                    }
                }
                return result;
            }
        }

        public string CacheConnectionString => _xmlFileSettings.Get(CacheConnectionStringKey);

        public bool SmtpEnabled => _xmlFileSettings.GetBool(SmtpEnabledKey, false);
        public string SmtpHost => _xmlFileSettings.Get(SmtpHostKey);
        public int SmtpPort => _xmlFileSettings.GetInt(SmtpPortKey, 0);
        public bool SmtpEnableSsl => _xmlFileSettings.GetBool(SmtpEnableSslKey, false);
        public string SmtpUsername => _xmlFileSettings.Get(SmtpUserNameKey);
        public string SmtpPassword => _xmlFileSettings.Get(SmtpPasswordKey);
        public string SmtpDomain => _xmlFileSettings.Get(SmtpDomainKey);
        public string SmtpFrom => _xmlFileSettings.Get(SmtpFromKey);

        public int AdoNetBatchSize => AppSettings.GetValue<int>("ADONetBatchSize");

        public bool ShowSql
        {
            get
            {
                var showSqlStr = _xmlFileSettings.Get(ShowSqlKey);
                if (!string.IsNullOrWhiteSpace(showSqlStr))
                {
                    return bool.Parse(showSqlStr);
                }
                return false;
            }
            set
            {
                _xmlFileSettings.Set(ShowSqlKey, value.ToString());
            }
        }


        void IRohamConfigsUpdater.SetInstall(bool installed)
        {
            _xmlFileSettings.Set(AppInstalledKey, installed.ToString());
        }

        void IRohamConfigsUpdater.SetAppName(string name)
        {
            _xmlFileSettings.Set(AppNameKey, name ?? "");
        }

        void IRohamConfigsUpdater.SetDatabase(string dbProvider, string connectionString)
        {
            var keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(DatabaseProviderKey, dbProvider),
                new KeyValuePair<string, string>(ConnectionStringKey, connectionString)
            };
            _xmlFileSettings.SetCollection(keyValues);
        }

        void IRohamConfigsUpdater.SetCacheProvider(string cacheProvider, string connectionString)
        {
            var keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(CacheProviderKey, cacheProvider),
                new KeyValuePair<string, string>(CacheConnectionStringKey, connectionString),
            };
            _xmlFileSettings.SetCollection(keyValues);
        }

        void IRohamConfigsUpdater.SetSmtp(bool enabled, string host, int port, bool enableSsl, string username, string password, string domain, string smtpFrom)
        {
            var keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(SmtpEnabledKey, enabled.ToString()),
                new KeyValuePair<string, string>(SmtpHostKey, host??""),
                new KeyValuePair<string, string>(SmtpPortKey, port.ToString()),
                new KeyValuePair<string, string>(SmtpEnableSslKey, enableSsl.ToString()),
                new KeyValuePair<string, string>(SmtpUserNameKey, username??""),
                new KeyValuePair<string, string>(SmtpPasswordKey, password ?? ""),
                new KeyValuePair<string, string>(SmtpDomainKey, domain ?? ""),
                new KeyValuePair<string, string>(SmtpFromKey, smtpFrom ?? "")
            };
            _xmlFileSettings.SetCollection(keyValues);
        }
    }
}
