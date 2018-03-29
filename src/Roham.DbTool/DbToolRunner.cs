using System;
using Roham.Lib.Logger;
using Roham.Lib.Domain.Persistence;
using Roham.Data;
using Roham.Data.DbUp;
using Roham.Data.DbUp.PreProcessors;
using Roham.Lib.Ioc;

namespace Roham.DbTool
{
    public interface IDbToolRunner
    {
        int Run(DbOptions dbOptions);
    }

    [AutoRegister]
    public class DbToolRunner : IDbToolRunner
    {
        private readonly static ILogger Log = LoggerFactory.GetLogger<DbToolRunner>();
        private readonly IDatabaseProviderFactory _dbProviderFactory;

        public DbToolRunner(IDatabaseProviderFactory dbProviderFactory)
        {
            _dbProviderFactory = dbProviderFactory;
        }

        public int Run(DbOptions options)
        {
            var dbInfo = options.DatabaseInfo;
            IDatabaseProvider databaseProvider = _dbProviderFactory.Create(dbInfo.DbProvider);            

            string dbName = $@"{options.DatabaseInfo.DataSource}\{options.DatabaseInfo.InitialCatalog}";

            if (options.Command.HasFlag(DbOptionCommands.Drop))
            {
                if (UserConfirmed(options))
                {
                    (databaseProvider as IDatabaseDDLProvider).DropDatabase(dbInfo);
                    LogInfo($"Database {dbName} dropped");
                }
                else
                {
                    return 1;
                }
            }

            if (options.Command.HasFlag(DbOptionCommands.Create))
            {
                (databaseProvider as IDatabaseDDLProvider).CreateDatabase(dbInfo);
                LogInfo($"Database {dbName} created");
            }

            if (options.Command.HasFlag(DbOptionCommands.Upgrade))
            {
                if (!Upgrade(options))
                {
                    return 1;
                }
                LogInfo($"Database {dbName} upgraded from {options.UpgradeScriptsPath}");
            }

            if (options.Command.HasFlag(DbOptionCommands.ExportSchema))
            {
                var persistenceMetaContextFactory = DbToolDependencyResolver.Current.Resolve<IPersistenceMetaContextFactory>();
                var persistenceMetContxt = persistenceMetaContextFactory.CreateMeta();

                string connString = databaseProvider.BuildConnectionString(dbInfo);
                persistenceMetContxt.ExportSchemaScript(connString, options.ExportScriptFileName);

                LogInfo("Database schema script exported successfully");
                Log.Info($"Database {dbName} schema script exported at {options.ExportScriptFileName}");
            }

            return 0;
        }

        private bool Upgrade(DbOptions options)
        {
            UpgradeEngine upgradeBuilder = new UpgradeEngineBuilder()
                .WithDatabase(options.DatabaseInfo)
                .WithScriptsFromFileSystem(options.UpgradeScriptsPath)
                .WithPreprocessor(new DefaultScriptPreProcessor(null))
                .JournalToTable()
                .WithDefaultSchema("dbo")
                .ShowSql(options.ShowSql)
                .LogScriptOutput(options.IsDebugMode)
                .WithExecutionTimeout(null)
                .WithTransaction()
                .LogToConsole()
                .Build(_dbProviderFactory);

            UpgradeResult result = upgradeBuilder.PerformUpgrade();
            if (result.Error != null)
            {
                Log.Error("Database upgrade failed", result.Error);
            }
            return result.Successful;
        }

        private bool UserConfirmed(DbOptions options)
        {
            bool dropConfirmed = options.IsForce;
            if (!dropConfirmed)
            {
                ConsoleLog.Write(ConsoleColor.White,
                    $"{options.DatabaseInfo.DataSource}\\{options.DatabaseInfo.InitialCatalog} database is going to be deleted, are you sure to drop the database");
                ConsoleLog.Write(ConsoleColor.Yellow, " [Y/N]? ");
                var answer = Console.ReadLine();
                Console.WriteLine();
                if (answer != null && answer.Trim().ToUpper() == "Y")
                {
                    dropConfirmed = true;
                }
            }
            return dropConfirmed;
        }

        private void LogInfo(string message)
        {
            Log.Info(message);
            ConsoleLog.WriteInfo(message);
        }
    }
}