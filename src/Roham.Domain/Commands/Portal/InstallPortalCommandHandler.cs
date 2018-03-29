using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Logger;
using Roham.Lib.Cryptography;
using Roham.Data;
using Roham.Data.DbUp.Output;
using Roham.Data.DbUp;
using Roham.Data.DbUp.PreProcessors;
using Roham.Domain.Configs;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Commands.Portal;
using Roham.Contracts.Commands.User;
using Roham.Contracts.Commands.Site;
using Roham.Contracts.Commands.Zone;
using Roham.Domain.Exceptions;
using Roham.Resources;
using Roham.Lib.Ioc;
using Roham.Lib.Emails;

namespace Roham.Domain.Commands.Portal
{
    [AutoRegister]
    public class InstallPortalCommandHandler : AbstractCommandHandler<InstallPortalCommand>
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<InstallPortalCommandHandler>();

        private readonly IRohamConfigs _rohamConfigs;
        private readonly IDatabaseProviderFactory _dbProviderFactory;        
        private readonly ICommandDispatcher _commandDispatcher;

        public InstallPortalCommandHandler(
           Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver,
           IRohamConfigs rohamConfigs,           
           IDatabaseProviderFactory dbProviderFactory,
           ICacheProvider cacheProvider,
           ICommandDispatcher commandDispatcher) : base(uowFactoryResolver)
        {
            _rohamConfigs = rohamConfigs;
            _dbProviderFactory = dbProviderFactory;
            CacheProvider = cacheProvider;
            _commandDispatcher = commandDispatcher;
        }

        protected ICacheProvider CacheProvider { get; }

        protected override void OnHandle(InstallPortalCommand command)
        {
            if (_rohamConfigs.IsConfigFileMissing)
            {
                throw new ConfigurationException("Roham config file is missing", ErrorMessages.ConfigFileMissing);
            }

            CheckCacheProvider(command);
            if (command.UseSmtp)
            {   
                CheckSmtpConnection(command.SmtpSettings);
            }

            var dbInfo = ExtractDatabaseInfo(command);
            var databaseCreated = false;
            try
            {   
                ModifyConfigAsInstalled(command);                
                CreateDatabase(dbInfo);
                databaseCreated = true;
                InitializeDatabase(command, dbInfo);                
                InitializePortal(command);
            }
            catch (Exception)
            {
                try
                {
                    if (databaseCreated)
                    {
                        DropDatabase(dbInfo);
                    }
                }
                finally
                {
                    ModifyConfigAsUnInstalled(command);
                }
                throw;
            }
        }

        private void CheckCacheProvider(InstallPortalCommand command)
        {
            var cacheProvider = CacheProviders.Memory;
            var cacheProviderName = command.CacheProviderName;
            if (!Enum.TryParse(cacheProviderName, out cacheProvider))
            {
                throw new CacheExeption($"{command.CacheProviderName} cache provider is not supported",
                                        ErrorMessages.CacheProviderNotSupported.Fmt(command.CacheProviderName));
            }

            string errorMessage = "";
            if (!CacheProvider.TryConnect(cacheProvider, command.CacheConnectionString, out errorMessage))
            {
                throw new CacheExeption($"Cache with connection string: {command.CacheConnectionString} can not be connected",
                                        ErrorMessages.CacheServerCannotBeConnected);
            }
        }

        private void CheckSmtpConnection(SmtpSettings smtpSettings)
        {
            if (smtpSettings == null)
            {
                throw new NullReferenceException("Smtp Settings");
            }

            string errorMessage = "";
            if (!Email.TryPingHost(smtpSettings.Host, smtpSettings.Port, out errorMessage))
            {
                throw new CacheExeption($"Email server {smtpSettings} can not be connected", ErrorMessages.EmailServerCannotBeConnected);
            }
        }

        private DatabaseInfo ExtractDatabaseInfo(InstallPortalCommand command)
        {
            // Since Roham has been not installed yet, no database exists 
            // and hence PersistenceUnitOfWork must not be used here
            DbProviders dbProvider;
            if (!Enum.TryParse(command.DatabaseProviderName, out dbProvider))
            {
                throw new DatabaseException($"{command.DatabaseProviderName} database provider is not supported",
                                            ErrorMessages.DataProviderNotSupported.Fmt(command.DatabaseProviderName));
            }

            var dbInfo = new DatabaseInfo(dbProvider, command.ConnectionString);
            string errorMsg = "";
            if (!dbInfo.Validate(out errorMsg))
            {
                throw new DatabaseException($"Database is invalid: {errorMsg}", ErrorMessages.DatabaseSettingsInvalid);
            }
            return dbInfo;
        }

        private void CreateDatabase(DatabaseInfo dbInfo)
        {  
            var databaseProvider = _dbProviderFactory.Create(dbInfo.DbProvider);
            try
            {
                (databaseProvider as IDatabaseDDLProvider).CreateDatabase(dbInfo);
            }
            catch(Exception ex)
            {
                throw new DatabaseException($"Database {dbInfo} cannot be created", ErrorMessages.DatabaseCreationFailed, ex);
            }

            // check database connection
            string errorMsg;
            if (!databaseProvider.TryConnect(databaseProvider.BuildConnectionString(dbInfo), out errorMsg))
            {
                throw new DatabaseException($"Database created, but database connection cannot be established: {errorMsg}", ErrorMessages.DatabaseConnectionFailed);
            }
        }

        private void DropDatabase(DatabaseInfo dbInfo)
        {
            var databaseProvider = _dbProviderFactory.Create(dbInfo.DbProvider);
            (databaseProvider as IDatabaseDDLProvider).DropDatabase(dbInfo);
        }

        private void ModifyConfigAsInstalled(InstallPortalCommand command)
        {
            var rohamConfigsModifier = _rohamConfigs as IRohamConfigsUpdater;

            rohamConfigsModifier.SetDatabase(command.DatabaseProviderName, command.ConnectionString);
            rohamConfigsModifier.SetCacheProvider(command.CacheProviderName, command.CacheConnectionString);
            if (command.UseSmtp)
            {
                var s = command.SmtpSettings;
                rohamConfigsModifier.SetSmtp(command.UseSmtp, s.Host, s.Port, s.EnableSsl, s.UserName, s.Password, s.Domain, command.SmtpFrom);
            }
            else
            {
                rohamConfigsModifier.SetSmtp(false, "", 25, false, "", "", "", null);
            }
            rohamConfigsModifier.SetInstall(true);
            rohamConfigsModifier.SetAppName(command.PortalName);
        }

        private void ModifyConfigAsUnInstalled(InstallPortalCommand command)
        {
            var rohamConfigsModifier = _rohamConfigs as IRohamConfigsUpdater;

            rohamConfigsModifier.SetInstall(false);
            rohamConfigsModifier.SetAppName("");
            rohamConfigsModifier.SetSmtp(false, "", 25, false, "", "", "", null);
            rohamConfigsModifier.SetCacheProvider("", "");
            rohamConfigsModifier.SetDatabase("", "");
        }

        private void InitializeDatabase(InstallPortalCommand command, DatabaseInfo dbInfo)
        {
            var upgradeLog = new UpgradeStringLog();
            var upgradeBuilder = new UpgradeEngineBuilder()
                .WithDatabase(dbInfo)
                .WithScriptsEmbeddedInAssembly(typeof(DbScripts.TestType).Assembly, _ => true)
                .WithPreprocessor(new DefaultScriptPreProcessor(null))
                .JournalToTable()
                .WithDefaultSchema("dbo")
                .ShowSql(_rohamConfigs.ShowSql)
                .LogScriptOutput(false)
                .WithExecutionTimeout(null)
                .WithTransaction()
                .LogTo(upgradeLog)
                .Build(_dbProviderFactory);

            UpgradeResult result = upgradeBuilder.PerformUpgrade();
            command.UpgradeOutput = upgradeLog.GetOutput();
            if (result.Error != null)
            {
                Log.Error("Database upgrade failed", result.Error);
                throw new DatabaseException("Database upgrade failed", ErrorMessages.DatabaseInitializationFailed, result.Error);
            }
        }

        private void InitializePortal(InstallPortalCommand command)
        {
            // Here, database created and initialize, hence UnitOfWork is used here
            using (var uow = UowFactory.CreateWithTransaction(IsolationLevel.ReadCommitted))
            {
                // Add system admin user
                var sysAdminRole = uow.Context
                    .Query<Entities.Security.Role>()
                    .Where(r => r.RoleType == RoleTypeCodes.SystemAdmin && r.IsSystemRole)
                    .SingleOrDefault();
                
                _commandDispatcher.Send(
                    new AddUserCommand
                    {
                        UserName = command.AdminUserName,
                        Email = command.AdminUserName,                        
                        IsSystemUser = true,
                        RoleIds = new List<long> { sysAdminRole.Id },
                    });

                var sysAdminUser = uow.Context
                    .Query<Entities.Security.User>()
                    .Single(u => u.IsSystemUser && u.UserName == command.AdminUserName);

                _commandDispatcher.Send(
                    new ChangePasswordCommand
                    {
                        UserId = sysAdminUser.Id,
                        PasswordHashAlgorithm = HashAlgorithm.PBKDF2.ToString(),
                        NewPasswordHash = HashUtil.Hash(HashAlgorithm.PBKDF2, command.AdminPassword)
                    });

                _commandDispatcher.Send(
                    new SetUserEmailConfirmCommand
                    {
                        UserId = sysAdminUser.Id,
                        Confirmed = false,
                    });

                // Add portal  
                uow.Context
                    .Add(new Entities.Sites.Portal
                    {
                        Name = command.PortalName,
                        Title = command.PortalName,
                        Owner = sysAdminUser
                    });

                // Add default site
                _commandDispatcher.Send(
                    new AddSiteCommand
                    {
                        SiteTitle = command.SiteName,
                        IsDefault = true,
                        IsActive = true,
                        IsPublic = true,
                        OwnerUsername = command.AdminUserName,
                    });

                string defaultSiteName = command.SiteName;
                var site = uow.Context
                    .Query<Entities.Sites.Site>()
                    .Single(s => s.Name == defaultSiteName);

                // Add default site zones
                foreach (var z in command.SiteZones)
                {
                    _commandDispatcher.Send(
                        new AddZoneCommand
                        {
                            SiteId = site.Id,
                            Title = z.Title,
                            ZoneType = z.Code,
                            IsActive = true,
                            IsPublic = true,
                            Description = z.Description,
                        });
                }

                uow.Complete();
            }
        }
    }
}
