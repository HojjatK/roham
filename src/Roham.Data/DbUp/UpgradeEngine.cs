using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Roham.Lib.Logger;
using Roham.Data.DbUp.ScriptExecutor;

namespace Roham.Data.DbUp
{
    public class UpgradeEngine
    {
        // TODO: use a logger adatper instead?
        private static readonly ILogger Log = LoggerFactory.GetLogger<UpgradeEngine>();

        public UpgradeEngine(IUpgradeConfiguration upgradeConfig)
        {
            Config = upgradeConfig;
        }

        private IUpgradeConfiguration Config { get; set; }

        public UpgradeResult PerformUpgrade()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("PerformUpgrade called");
            }

            var executed = new List<SqlScript>();
            IDbTransaction tranx = null;
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    var connectionFactory = SetupConnectionFactory(conn);

                    if (Config.TransactionMode == TransactionMode.SingleTransaction)
                    {
                        tranx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                        connectionFactory.SetCurrentTransaction(tranx);
                    }

                    var scriptsToExecute = GetScriptsToExecuteInsideOperation(Config.UpgradeJournal).ToList();
                    if (scriptsToExecute.Count == 0)
                    {
                        Log.Info("No new scripts found");
                        Config.UpgradeLog.WriteInfo("No new scripts need to be executed - completing.");
                        return new UpgradeResult(executed, true, null);
                    }

                    Config.ScriptExecutor.VerifySchema();
                    scriptsToExecute.ForEach(script =>
                    {
                        IDbTransaction sTranx = null;
                        try
                        {
                            if (Config.TransactionMode == TransactionMode.TransactionPerScript)
                            {
                                sTranx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                                connectionFactory.SetCurrentTransaction(sTranx);
                            }
                            Config.ScriptExecutor.Execute(script, Config.Variables);
                            Config.UpgradeJournal.StoreExecutedScript(script);
                            executed.Add(script);
                            if (sTranx != null)
                            {
                                sTranx.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            Rollback(sTranx);
                            throw;
                        }

                    });

                    if (tranx != null)
                    {
                        tranx.Commit();
                    }
                    Log.Debug("PerformUpgrade succeed");
                    return new UpgradeResult(executed, true, null);
                }

            }
            catch (Exception ex)
            {
                Rollback(tranx);                
                Log.Error($@"Upgrade failed due to an unexpected exception:\r\n{ex}");
                return new UpgradeResult(executed, false, ex);
            }
        }

        ///<summary>
        /// Creates version record for any new migration scripts without executing them.
        /// Useful for bringing development environments into sync with automated environments
        ///</summary>        
        public UpgradeResult MarkAsExecuted()
        {
            var marked = new List<SqlScript>();

            try
            {
                List<SqlScript> scriptsToExecute;
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    var connectionFactory = SetupConnectionFactory(conn);

                    scriptsToExecute = GetScriptsToExecuteInsideOperation(Config.UpgradeJournal).ToList();
                    scriptsToExecute.ForEach(script =>
                    {
                        Config.UpgradeJournal.StoreExecutedScript(script);

                        var infMsg = $"Marking script {script.Name} as executed";
                        Log.Info(infMsg);
                        Config.UpgradeLog.WriteInfo(infMsg);

                        marked.Add(script);
                    });
                }

                Log.Info("Script marking successful");
                return new UpgradeResult(marked, true, null);
            }
            catch (Exception ex)
            {
                Log.Error($@"Upgrade failed due to an unexpected exception:\r\n{ex}");
                return new UpgradeResult(marked, false, ex);
            }
        }

        private IEnumerable<SqlScript> GetScriptsToExecuteInsideOperation(IUpgradeJournal upgradeJournal)
        {
            var allScripts = Config.ScriptProvider.GetScripts();
            var executedScripts = upgradeJournal.GetExecutedScripts();
            return allScripts.Where(s => !executedScripts.Any(y => y == s.Name)).ToList();
        }

        private IDbConnection CreateConnection()
        {
            var dbInfo = Config.DatabaseInfo;
            var databaseProvider = Config.DatabaseProviderFactory.Create(dbInfo.DbProvider);
            var connFactory = databaseProvider.GetConnectionFactory(databaseProvider.BuildConnectionString(dbInfo));
            var conn = connFactory();
            return conn;
        }

        private ConnectionFactory SetupConnectionFactory(IDbConnection conn)
        {
            var connectionFactory = new ConnectionFactory(
                () => conn,
                (c, t) =>
                {
                    var cmd = c.CreateCommand();
                    if (t != null)
                    {
                        cmd.Transaction = t;
                    }
                    return cmd;
                });
            var scriptExecutor = Config.ScriptExecutor;
            if (scriptExecutor is IConnectionFactoryHolder)
            {
                (scriptExecutor as IConnectionFactoryHolder).ConnectionFactory = connectionFactory;
            }
            var upgradeJournal = Config.UpgradeJournal;
            if (upgradeJournal is IConnectionFactoryHolder)
            {
                (upgradeJournal as IConnectionFactoryHolder).ConnectionFactory = connectionFactory;
            }

            return connectionFactory;
        }

        private void Rollback(IDbTransaction trans)
        {
            if (trans != null)
            {
                try
                {
                    trans.Rollback();
                }
                catch (Exception ex)
                {
                    Log.Warn("Transaction could not be rolled back", ex);
                }
            }
        }
    }
}