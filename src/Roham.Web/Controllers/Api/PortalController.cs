using System.Web.Http;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Contracts.Commands.Portal;
using Roham.Domain.Settings;
using System.Collections.Generic;
using System;
using Roham.Domain.Configs;
using Roham.Data;
using Roham.Domain.Services;
using Roham.Lib.Emails;
using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Security;

namespace Roham.Web.Controllers.Api
{   
    [RoutePrefix("api/portal")]
    [Authorize(Roles = SecurityRoleNames.SysAdmin)]
    public class PortalController : ApiControllerBase
    {
        private readonly Func<IRohamConfigs> _rohamConfigsResolver;
        private readonly Func<ICacheProvider> _cacheProviderResolver;
        private readonly Func<ICacheService> _cacheServiceResolver;
        private ISettingsProvider SettingsProvider { get; }

        public PortalController(
            ISettingsProvider settingsProvider,
            Func<IRohamConfigs> rohamConfigsResolver,
            Func<ICacheProvider> cacheProviderResolver,
            Func<ICacheService> cacheServiceResolver,
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher)
        {
            _rohamConfigsResolver = rohamConfigsResolver;
            _cacheProviderResolver = cacheProviderResolver;
            _cacheServiceResolver = cacheServiceResolver;
            SettingsProvider = settingsProvider;
        }

        [HttpGet]
        [Route("")]
        public PortalDto GetPortal(bool @defaultSettigs = false)
        {
            var portalDto = QueryExecutor.Execute(new FindPortalQuery());

            var portalSettings = @defaultSettigs ?
                SettingsProvider.GetDefaultSettings<PortalSettings>(null) :
                SettingsProvider.GetSettings<PortalSettings>(null);
            portalDto.Settings = ConvertFrom(portalSettings);

            return portalDto;
        }

        [HttpGet]
        [Route("copyright")]
        public string GetCopyright()
        {
            return WebAppInfo.Copyright;
        }

        [HttpPut]
        [Route("")]
        public ResultDto UpdatePortal(PortalDto portalDto)
        {
            return Result(() =>
            {
                var command = new UpdatePortalCommand
                {
                    Name = portalDto.Title,
                    Title = portalDto.Title,
                    Description = portalDto.Description,
                    SettingsDto = portalDto.Settings,
                };
                CommandDispatcher.SendWithTransaction(command);
            });
        }

        [HttpGet]
        [Route("settings/smtp")]
        public PortalSmtpSettingsDto GetSmtpSettings()
        {
            var rohamConfigs = _rohamConfigsResolver();
            var result = new PortalSmtpSettingsDto
            {
                SmtpEnabled = rohamConfigs.SmtpEnabled,
                SmtpHost = rohamConfigs.SmtpHost,
                SmtpPort = rohamConfigs.SmtpPort,
                SmtpUsername = rohamConfigs.SmtpUsername,
                SmtpPassword = rohamConfigs.SmtpPassword,
                SmtpDomain = rohamConfigs.SmtpDomain,
                SmtpUseSsl = rohamConfigs.SmtpEnableSsl,
                SmtpFrom = rohamConfigs.SmtpFrom
            };

            return result;
        }

        [HttpPut]
        [Route("settings/smtp")]
        public ResultDto SaveSmtpSettings(PortalSmtpSettingsDto smtpSettingsDto)
        {
            return Result(() =>
            {
                var rohamConfigs = _rohamConfigsResolver();
                var rohamConfigsUpdater = rohamConfigs as IRohamConfigsUpdater;

                string errorMessage = "";
                var smtpSettings = smtpSettingsDto.ToSmtpSettings();
                if (smtpSettingsDto.SmtpEnabled)
                {
                    if (!Email.TryPingHost(smtpSettings.Host, smtpSettings.Port, out errorMessage))
                    {
                        throw new ValidationException($"Email server cannot be connected. {errorMessage}");
                    }
                }

                rohamConfigsUpdater.SetSmtp(smtpSettingsDto.SmtpEnabled, smtpSettings.Host, smtpSettings.Port, smtpSettings.EnableSsl,
                                            smtpSettings.UserName, smtpSettings.Password, smtpSettings.Domain, smtpSettingsDto.SmtpFrom);
            });
        }

        [HttpGet]
        [Route("settings/cache")]
        public PortalCacheSettingsDto GetCacheSettings()
        {
            var rohamConfigs = _rohamConfigsResolver();
            var cacheProvider = _cacheProviderResolver();

            string cacheHost = "", cachePassword = "";
            int cachePort = 0;
            bool cacheUseSsl = false;
            if (rohamConfigs.CacheProvider == CacheProviders.Redis)
            {
                var cacheInfo = cacheProvider.CreateInfo(CacheProviders.Redis, rohamConfigs.CacheConnectionString);
                cacheHost = cacheInfo.Host;
                cachePassword = cacheInfo.Password;
                cachePort = cacheInfo.Port;
                cacheUseSsl = cacheInfo.Ssl;
            }

            var result = new PortalCacheSettingsDto
            {
                ExtCacheEnabled = rohamConfigs.CacheProvider == CacheProviders.Redis,
                CacheProvider = rohamConfigs.CacheProvider.ToString(),
                CacheHost = cacheHost,
                CachePort = cachePort,
                CachePassword = cachePassword,
                CacheUseSsl = cacheUseSsl,
                AvailableCacheProviders = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(CacheProviders.Memory.ToString(), "Memory"),
                    new KeyValuePair<string, string>(CacheProviders.Redis.ToString(), "Redis"),
                }
            };

            return result;
        }

        [HttpPut]
        [Route("settings/cache")]
        public ResultDto SaveCacheSettings(PortalCacheSettingsDto cacheSettingsDto)
        {
            return Result(() =>
            {
                var rohamConfigs = _rohamConfigsResolver();
                var rohamConfigsUpdater = rohamConfigs as IRohamConfigsUpdater;

                CacheProviders provider;
                string cacheConnectionString = "";
                
                var cacheInfo = GetCacheInfo(cacheSettingsDto, out provider);
                TestCacheConnection(cacheInfo, out cacheConnectionString);                
                rohamConfigsUpdater.SetCacheProvider(provider.ToString(), cacheConnectionString);

                // Clear cache if required                                
                _cacheServiceResolver().Refresh();
            });
        }

        [HttpPost]
        [Route("settings/check-cache")]
        public ResultDto CheckCache(PortalCacheSettingsDto cacheSettingsDto)
        {
            return Result(() =>
            {
                CacheProviders provider;
                string cacheConnectionString = "";
                var cacheInfo = GetCacheInfo(cacheSettingsDto, out provider);
                TestCacheConnection(cacheInfo, out cacheConnectionString);
            });
        }

        [HttpPost]
        [Route("settings/reset-cache")]
        public ResultDto ResetCache(PortalCacheSettingsDto cacheSettingsDto)
        {
            return Result(() =>
            {
                CacheProviders provider;
                string cacheConnectionString = "";
                var cacheInfo = GetCacheInfo(cacheSettingsDto, out provider);
                TestCacheConnection(cacheInfo, out cacheConnectionString);

                _cacheServiceResolver().Refresh();
            });
        }

        private CacheInfo GetCacheInfo(PortalCacheSettingsDto cacheSettingsDto, out CacheProviders provider)
        {
            provider = CacheProviders.Memory;
            if (!Enum.TryParse(cacheSettingsDto.CacheProvider, out provider))
            {
                throw new ValidationException($"{provider} is not supported");
            }

            CacheInfo result = null;
            if (provider == CacheProviders.Redis)
            {
                var cacheProvider = _cacheProviderResolver();
                if (string.IsNullOrWhiteSpace(cacheSettingsDto.CacheHost))
                {
                    throw new ValidationException("Host cannot be an empty string");
                }
                result = new CacheInfo(CacheProviders.Redis, cacheSettingsDto.CacheHost, cacheSettingsDto.CachePort,
                                       cacheSettingsDto.CacheUseSsl, cacheSettingsDto.CachePassword);                
            }
            else
            {
                result = new CacheInfo(CacheProviders.Memory, "", 0);
            }
            return result;
        }

        private void TestCacheConnection(CacheInfo cacheInfo, out string cacheConnectionString)
        {
            cacheConnectionString = "";
            if (cacheInfo.Provider == CacheProviders.Redis)
            {
                var cacheProvider = _cacheProviderResolver();
                if (string.IsNullOrWhiteSpace(cacheInfo.Host))
                {
                    throw new ValidationException("Host cannot be an empty string");
                }
                cacheConnectionString = cacheProvider.BuildConnectionString(cacheInfo);
                var errorMessage = "";
                if (!cacheProvider.TryConnect(cacheInfo, out errorMessage))
                {
                    throw new ValidationException($"Cache server cannot be connected. {errorMessage}");
                }
            }
        }

        private PortalSettingsDto ConvertFrom(PortalSettings portalSettings)
        {
            return new PortalSettingsDto
            {
                AvailableStorageProviders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string> ("filesystem","File System"),
                    new KeyValuePair<string, string> ("cloud", "Cloud"),
                },
                StorageProvider = portalSettings.StorageProvider,
                UploadPath = portalSettings.UploadPath,
                StorageConnectionString = portalSettings.StorageConnectionString,
                BlobContainerName = portalSettings.BlobContainerName,
                AvailableThemes = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("default", "Default"),
                    new KeyValuePair<string, string>("dark", "Dark"),
                    new KeyValuePair<string, string>("blue", "Blue"),
                },
                AdminTheme = portalSettings.AdminTheme
            };
        }

    }
}
