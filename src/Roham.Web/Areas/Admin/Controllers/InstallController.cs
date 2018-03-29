using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Roham.Lib.Logger;
using Roham.Lib.Domain.CQS.Command;
using Roham.Data;
using Roham.Domain.Configs;
using Roham.Web.Areas.Admin.ViewModels;
using Roham.Contracts.Commands.Portal;
using Roham.Web.Mvc.Filters;
using Roham.Resources;
using Roham.Domain.Exceptions;
using Roham.Domain.Services;
using Roham.Lib.Caches;
using Roham.Lib.Emails;

namespace Roham.Web.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [LogActions]
    public class InstallController : Controller
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<InstallController>();
        private static readonly object @intallLock = new object();

        private readonly IRohamConfigs _rohamConfigs;
        private readonly ICacheService _cacheService;
        private readonly IDatabaseProviderFactory _dbProviderFactory;
        private readonly ICommandDispatcher _commandDispatcher;

        public InstallController(
            IRohamConfigs rohamConfigs,
            ICacheService cacheService,
            ICacheProvider cacheProvider,
            IDatabaseProviderFactory dbProviderFactory,
            ICommandDispatcher commandDispatcher)
        {
            _rohamConfigs = rohamConfigs;
            _cacheService = cacheService;
            _dbProviderFactory = dbProviderFactory;
            _commandDispatcher = commandDispatcher;
            CacheProvider = cacheProvider;
        }

        protected ICache MemoryCache => _cacheService.MemoryCache;
        protected ICacheProvider CacheProvider { get; }

        [HttpGet]
        [Route("install")]
        [Route("install/{uid}")]        
        public ActionResult Index(string uid = null)
        {
            if (!_rohamConfigs.IsInstalled)
            {
                var dbInfo = new DatabaseInfo(DbProviders.SqlServer, "localhost", WebAppInfo.Name, "", "", true);
                var vm = new InstallViewModel
                {
                    SelectedDatabaseProvider = dbInfo.DbProvider.ToString(),
                    IsInstalled = false,
                    SqlServer = new SqlServerViewModel
                    {
                        ConnectionString = _dbProviderFactory.Create(dbInfo.DbProvider).BuildConnectionString(dbInfo),
                        DatabaseServer = dbInfo.DataSource,
                        DatabaseName = dbInfo.InitialCatalog,
                        Username = dbInfo.UserName,
                        Password = dbInfo.Password,
                    },
                    SelectedCacheProvider = CacheProviders.Memory.ToString(),                    
                    SelectedEmailProvider = "smtp",
                    CacheConfigs = null,
                    InstallationKey = "",
                };

                if (IsKeyVerified(uid))
                {
                    ViewBag.KeyVerified = true;
                }
                else if (uid != null)
                {
                    ModelState.AddModelError(nameof(ErrorMessages.InstallationKeyInvalid), ErrorMessages.InstallationKeyInvalid);
                }
                return View("Install", vm);
            }
            else
            {
                return RedirectToAction("upgrade");
            }
        }

        [HttpPost]
        [Route("install/verifykey")]        
        public ActionResult VerifyInstallationKey(InstallViewModel model)
        {
            if (model != null && model.InstallationKey == _rohamConfigs.InstallationPassword)
            {
                SetKeyVerified(model.Uid, true);
            }
            else
            {
                SetKeyVerified(model.Uid, false);
            }
            return RedirectToAction("", new { uid = model.Uid });
        }

        [HttpPost]
        [Route("install/dbprovider")]        
        public PartialViewResult GetDatabaseProviderPartialView(InstallViewModel model)
        {
            var dbProvider = DbProviders.SqlServer; // default 
            if (!string.IsNullOrEmpty(model.SelectedDatabaseProvider))
            {
                Enum.TryParse(model.SelectedDatabaseProvider, out dbProvider);
            }
            switch (dbProvider)
            {
                case DbProviders.SqlServer:
                    if (model.SqlServer == null)
                    {
                        var dbInfo = new DatabaseInfo(DbProviders.SqlServer, "localhost", "RohamDb", "", "", true);
                        model.SqlServer = new SqlServerViewModel
                        {
                            ConnectionString = _dbProviderFactory.Create(DbProviders.SqlServer).BuildConnectionString(dbInfo),
                            DatabaseServer = dbInfo.DataSource,
                            DatabaseName = dbInfo.InitialCatalog,
                            Username = dbInfo.UserName,
                            Password = dbInfo.Password,
                        };
                    }
                    else
                    {
                        model.SqlServer.ConnectionString = _dbProviderFactory.Create(DbProviders.SqlServer).BuildConnectionString(ExtractDatabaseInfo(model, model.Advanced));
                    }
                    return PartialView("~/Areas/Admin/Views/Install/_SqlServer.cshtml", model);
                case DbProviders.SQLite:
                    if (model.Sqlite == null)
                    {
                        var dbInfo = new DatabaseInfo(DbProviders.SqlServer, "Roham", "RohamDb", "", "", true);
                        model.Sqlite = new SqliteViewModel
                        {
                            ConnectionString = _dbProviderFactory.Create(DbProviders.SQLite).BuildConnectionString(dbInfo),
                            DataSource = dbInfo.DataSource,
                            Password = dbInfo.Password,
                        };
                    }
                    else
                    {
                        model.Sqlite.ConnectionString = _dbProviderFactory.Create(DbProviders.SQLite).BuildConnectionString(ExtractDatabaseInfo(model, model.Advanced));
                    }
                    return PartialView("~/Areas/Admin/Views/Install/_Sqlite.cshtml", model);
                default:
                    throw new DatabaseException($"{dbProvider} database provider not supported",
                                                ErrorMessages.DataProviderNotSupported.Fmt(dbProvider));
            }
        }

        [HttpPost]
        [Route("install/dbconnstr")]
        public string BuildDbConnectionString(InstallViewModel model)
        {
            string connectionString = "";
            var dbProvider = DbProviders.SqlServer; // default 
            if (!string.IsNullOrEmpty(model.SelectedDatabaseProvider))
            {
                Enum.TryParse(model.SelectedDatabaseProvider, out dbProvider);
            }
            switch (dbProvider)
            {
                case DbProviders.SqlServer:
                    if (model.SqlServer != null)
                    {
                        connectionString = _dbProviderFactory.Create(DbProviders.SqlServer).BuildConnectionString(ExtractDatabaseInfo(model, false));
                    }
                    break;
                case DbProviders.SQLite:
                    if (model.Sqlite != null)
                    {
                        connectionString = _dbProviderFactory.Create(DbProviders.SQLite).BuildConnectionString(ExtractDatabaseInfo(model, false));
                    }
                    break;
                default:
                    throw new DatabaseException($"{dbProvider} database provider not supported",
                                                ErrorMessages.DataProviderNotSupported.Fmt(dbProvider));
            }
            return connectionString;
        }

        [HttpPost]
        [Route("install/cacheprovider")]
        public PartialViewResult GetCacheProviderPartialView(InstallViewModel model)
        {
            var cacheProvider = CacheProviders.Memory; // default 
            if (!string.IsNullOrEmpty(model.SelectedCacheProvider))
            {
                Enum.TryParse(model.SelectedCacheProvider, out cacheProvider);
            }
            switch (cacheProvider)
            {
                case CacheProviders.Redis:
                    if (model.CacheConfigs == null)
                    {
                        string hostName = "localhost";
                        int port = 6379;
                        model.CacheConfigs = new CacheConfigsViewModel
                        {
                            Provider = CacheProviders.Redis,
                            Host = hostName,
                            Port = port,
                            ConnectionString = CacheProvider.BuildConnectionString(new CacheInfo(CacheProviders.Redis, hostName, port))
                        };
                    }
                    else
                    {
                        model.CacheConfigs.Provider = CacheProviders.Redis;
                        model.CacheConfigs.ConnectionString = CacheProvider.BuildConnectionString(ExtractCacheInfo(model, model.AdvancedCache));
                    }
                    return PartialView("~/Areas/Admin/Views/Install/_Cache.cshtml", model);
                case CacheProviders.Memory:
                    model.CacheConfigs = new CacheConfigsViewModel
                    {
                        Provider = CacheProviders.Memory,
                    };
                    model.CacheConfigs.ConnectionString = "";
                    return PartialView("~/Areas/Admin/Views/Install/_Cache.cshtml", model);
                default:
                    throw new DatabaseException($"{cacheProvider} cache provider not supported",
                                                ErrorMessages.CacheProviderNotSupported.Fmt(cacheProvider));
            }
        }

        [HttpPost]
        [Route("install/cacheconnstr")]
        public string BuildCacheConnectionString(InstallViewModel model)
        {
            string connectionString = "";
            var cacheProvider = CacheProviders.Memory; // default 
            if (!string.IsNullOrEmpty(model.SelectedCacheProvider))
            {
                Enum.TryParse(model.SelectedCacheProvider, out cacheProvider);
            }
            switch (cacheProvider)
            {
                case CacheProviders.Redis:
                    if (model.CacheConfigs != null)
                    {
                        connectionString = CacheProvider.BuildConnectionString(ExtractCacheInfo(model, false));
                    }
                    break;
                case CacheProviders.Memory:
                    connectionString = "";
                    break;
                default:
                    throw new DatabaseException($"{cacheProvider} cache provider not supported",
                                                ErrorMessages.CacheProviderNotSupported.Fmt(cacheProvider));
            }
            return connectionString;
        }

        [HttpPost]
        [Route("install/cacheconnect")]
        public ActionResult TestCacheConnect(InstallViewModel model)
        {
            var cacheProvider = CacheProviders.Memory;
            if (!string.IsNullOrEmpty(model.SelectedCacheProvider))
            {
                Enum.TryParse(model.SelectedCacheProvider, out cacheProvider);
            }

            var cacheConfig = model?.CacheConfigs;
            string errorMessage = "Cache cannot be connected";
            if (cacheConfig != null && cacheProvider == CacheProviders.Redis)
            {
                cacheConfig.Provider = cacheProvider;                
                var cacheInfo = ExtractCacheInfo(model, model.AdvancedCache);
                if (CacheProvider.TryConnect(cacheInfo, out errorMessage))
                {
                    return Content("");
                }
            }
            throw new CacheExeption(errorMessage, ErrorMessages.CacheServerCannotBeConnected);
        }

        [HttpPost]
        [Route("install/smtpconnect")]
        public ActionResult TestSmtpConnect(InstallViewModel model)
        {
            var smtpSettings = model?.Smtp;
            string errorMessage = "Email server settings invalid";
            if (smtpSettings != null)
            {   
                if (Email.TryPingHost(smtpSettings.Host, smtpSettings.Port, out errorMessage))
                {
                    return Content("");                    
                }
            }
            throw new EmailException(errorMessage, ErrorMessages.EmailServerCannotBeConnected);
        }

        [HttpPost]
        [Route("install/smtptestemail")]
        public ActionResult TestSendEmail(InstallViewModel model)
        {
            var smtpSettings = model?.Smtp;
            string errorMessage = "Email server settings invalid";
            if (smtpSettings != null)
            {
                if (Email.TryPingHost(smtpSettings.Host, smtpSettings.Port, out errorMessage))
                {
                    var settings = new SmtpSettings(smtpSettings.Host, smtpSettings.Port, smtpSettings.EnableSsl,
                                                      smtpSettings.UserName, smtpSettings.Password, smtpSettings.Domain);
                    var testEmail = new Email(settings, model.Smtp.TestEmailFromAddress, new List<string> { model.Smtp.TestEmailToAddress }, $"{WebAppInfo.Name} test email", "This is a test e-mail");
                    if (testEmail.TrySend(out errorMessage))
                    {
                        return Content("");
                    }
                    throw new EmailException(errorMessage, ErrorMessages.EmailCannotbeSent);
                }
                else
                {
                    throw new EmailException(errorMessage, ErrorMessages.EmailServerCannotBeConnected);
                }
            }
            throw new EmailException(errorMessage, ErrorMessages.EmailServerCannotBeConnected);
        }

        [HttpPost]
        [Route("install")]        
        public ActionResult Install(InstallViewModel model)
        {
            if (_rohamConfigs.IsInstalled)
            {
                // Redirect to upgrade is invalid and should not happen here
                throw new InvalidOperationException("Application is not installed yet");
            }

            if (model != null && IsKeyVerified(model.Uid))
            {
                ViewBag.KeyVerified = true;
            }
            if (!ModelState.IsValid || model == null)
            {
                return View("Install", model);
            }
            if (model.AdminPassword != model.ConfirmAdminPassword)
            {
                ModelState.AddModelError(nameof(ErrorMessages.PasswordAndConfirmShouldMatch),  ErrorMessages.PasswordAndConfirmShouldMatch);
                return View("Install", model);
            }

            try
            {
                var dbInfo = ExtractDatabaseInfo(model, model.Advanced);
                var cacheInfo = ExtractCacheInfo(model, model.AdvancedCache);
                var installCommand = new InstallPortalCommand
                {
                    DatabaseProviderName = model.SelectedDatabaseProvider,
                    CacheProviderName = model.SelectedCacheProvider,
                    ConnectionString = _dbProviderFactory.Create(dbInfo.DbProvider).BuildConnectionString(dbInfo),
                    CacheConnectionString = CacheProvider.BuildConnectionString(cacheInfo),                    
                    UseSmtp = model.UseSmtp,
                    SmtpFrom = model.SmtpFrom,
                    SmtpSettings = model.Smtp != null ? model.Smtp.ConvertToSettings() : null,
                    PortalName = model.PortalName,
                    AdminUserName = model.AdminUserName,
                    AdminPassword = model.AdminPassword,
                    SiteName = model.PortalName,
                    SiteZones = model.Zones.Where(z => model.SelectedZones.Contains(z.Name)).ToList()
                };

                lock (@intallLock)
                {
                    _commandDispatcher.Send(installCommand);
                }

                var upgradeVm = new UpgradeViewModel
                {
                    CacheKey = GetUpgradeCacheKey(model.Uid),
                    UpgradeOutput = installCommand.UpgradeOutput,
                    Message = $"{model.PortalName} installed successfully"
                };
                MemoryCache.Set(upgradeVm.CacheKey, upgradeVm);

                return RedirectToAction("Upgrade", new { uid = model.Uid });
            }
            catch (Exception ex)
            {
                Log.Warn("Roham installation failed", ex);

                var rohamExp = ex as RohamException;
                var errorMsg = !string.IsNullOrWhiteSpace(rohamExp?.DisplayMessage) ? rohamExp.DisplayMessage : ErrorMessages.InstallationFailed;
                ModelState.AddModelError(nameof(ErrorMessages.InstallationFailed), errorMsg);

                return View("Install", model);
            }
        }

        [HttpGet]
        [Route("upgrade")]        
        public ActionResult Upgrade(string uid = null)
        {
            if (!_rohamConfigs.IsInstalled)
            {
                // Redirect to upgrade is invalid and should not happen 
                throw new InvalidOperationException("Application is not installed yet");
            }

            var vm = new UpgradeViewModel
            {
                UpgradeOutput = new List<string>()
            };
            if (uid != null)
            {
                MemoryCache.TryGet(GetUpgradeCacheKey(uid), out vm);
            }

            return View("Upgrade", vm);
        }

        private bool IsKeyVerified(string uid)
        {
            var key = GetKeyVerifiedCacheKey(uid);
            bool value = false;
            if (key != null && MemoryCache.TryGet(key, out value))
            {
                return value;
            }
            return false;
        }

        private void SetKeyVerified(string uid, bool value)
        {
            var key = GetKeyVerifiedCacheKey(uid);
            if (key != null)
            {   
                MemoryCache.Set(key, value);
            }
        }

        private string GetKeyVerifiedCacheKey(string uid)
        {
            if (uid == null)
            {
                return null;
            }
            return string.Format("{0}-{1}", uid, "KeyVerified");
        }

        private string GetUpgradeCacheKey(string uid)
        {
            if (uid == null)
            {
                return null;
            }
            return $"{uid}-UpgradeVm";
        }

        private DatabaseInfo ExtractDatabaseInfo(InstallViewModel model, bool fromConnectionString)
        {
            DbProviders selectedDbProvider = DbProviders.None;
            Enum.TryParse(model.SelectedDatabaseProvider, out selectedDbProvider);
            var providerFactory = _dbProviderFactory.Create(selectedDbProvider);

            DatabaseInfo dbInfo = null;
            switch (selectedDbProvider)
            {
                case DbProviders.SqlServer:                    
                    if (fromConnectionString)
                    {
                        dbInfo = new DatabaseInfo(DbProviders.SqlServer, model.SqlServer.ConnectionString);
                        model.SqlServer.DatabaseServer = dbInfo.DataSource;
                        model.SqlServer.DatabaseName = dbInfo.InitialCatalog;
                        if (!string.IsNullOrWhiteSpace(dbInfo.UserName) && !string.IsNullOrWhiteSpace(dbInfo.Password))
                        {
                            model.SqlServer.IntegratedSecurity = true;
                            model.SqlServer.Username = dbInfo.UserName;
                            model.SqlServer.Password = dbInfo.Password;
                        }
                    }
                    else
                    {
                        dbInfo = new DatabaseInfo(DbProviders.SqlServer,
                            model.SqlServer.DatabaseServer,
                            model.SqlServer.DatabaseName,
                            model.SqlServer.IntegratedSecurity ? null : model.SqlServer.Username,
                            model.SqlServer.IntegratedSecurity ? null : model.SqlServer.Password, 
                            false);
                    }
                    break;
                case DbProviders.SQLite:                    
                    if (model.Advanced)
                    {
                        dbInfo = new DatabaseInfo(DbProviders.SQLite, model.Sqlite.ConnectionString);
                        model.Sqlite.DataSource = dbInfo.DataSource;
                        model.Sqlite.Password = dbInfo.Password;
                    }
                    else
                    {
                        dbInfo = new DatabaseInfo(DbProviders.SQLite, model.Sqlite.DataSource, model.Sqlite.DataSource, null, model.Sqlite.Password, true);
                    }
                    break;
                default:
                    throw new DatabaseException($"{model.SelectedDatabaseProvider} database provider not supported",
                                                ErrorMessages.DataProviderNotSupported.Fmt(model.SelectedDatabaseProvider));
            }
            return dbInfo;
        }

        private CacheInfo ExtractCacheInfo(InstallViewModel model, bool fromConnectionString)
        {
            var selectedCacheProvider = CacheProviders.Memory;
            Enum.TryParse(model.SelectedCacheProvider, out selectedCacheProvider);

            var cacheConfigs = model?.CacheConfigs;
            if (cacheConfigs == null)
            {
                cacheConfigs = new CacheConfigsViewModel();
            }
            cacheConfigs.Provider = selectedCacheProvider;

            CacheInfo cacheInfo = null;
            switch(selectedCacheProvider)
            {
                case CacheProviders.Memory:
                    cacheInfo = new CacheInfo(CacheProviders.Memory);
                    break;
                case CacheProviders.Redis:
                    if (fromConnectionString)
                    {   
                        cacheInfo = CacheProvider.CreateInfo(CacheProviders.Redis, model.CacheConfigs.ConnectionString);
                        model.CacheConfigs.Host = cacheInfo.Host;
                        model.CacheConfigs.Port = cacheInfo.Port;
                        model.CacheConfigs.Ssl = cacheInfo.Ssl;
                    }
                    else
                    {
                        cacheInfo = new CacheInfo(CacheProviders.Redis, cacheConfigs.Host, cacheConfigs.Port, cacheConfigs.Ssl, cacheConfigs.Password);
                    }
                    break;
                default:
                    throw new CacheExeption($"{model.SelectedCacheProvider} cache provider not supported",
                                            ErrorMessages.CacheProviderNotSupported.Fmt(model.SelectedCacheProvider));
            }
            return cacheInfo;
        }
    }
}