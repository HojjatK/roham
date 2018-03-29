using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Util;
using Roham.Lib.Domain.Persistence;
// TODO: replace miniprofiling with ?
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace Roham.Persistence.NHibernate.Configurators
{
    internal class SqlPersistenceProviderConfigurator : PersistenceProviderBase
    {
        public SqlPersistenceProviderConfigurator(Assembly mappingsAssembly, IPersistenceConfigs configs) : base(mappingsAssembly, configs) { }

        protected override IPersistenceConfigurer GetProviderConfigurer(string connectionString)
        {
            var c = MsSqlConfiguration.MsSql2008
                .ConnectionString(connectionString)
                //.Driver<ProfiledSql2008ClientDriver>() // causes lock recursion
                .Driver<Sql2008ClientDriver>()
                .UseReflectionOptimizer();

            //if (!string.IsNullOrEmpty(_configs.Schema))
            //    c.DefaultSchema(_configs.Schema);

            if (_configs.AdoNetBatchSize > 0)
            {
                c.AdoNetBatchSize(_configs.AdoNetBatchSize);
            }
            else
            {
                c.AdoNetBatchSize(200);
            }

            if (_configs.ShowSql)
            {
                c.ShowSql();
            }

            return c;
        }

        #region Nested Classes

        internal class ProfiledSql2008ClientDriver : Sql2008ClientDriver, IEmbeddedBatcherFactoryProvider
        {
            public override IDbConnection CreateConnection()
            {
                return new ProfiledSqlDbConnection(
                    new SqlConnection(),
                    MiniProfiler.Current);
            }

            public override IDbCommand CreateCommand()
            {
                return new ProfiledSqlDbCommand(
                    new SqlCommand(),
                    null,
                    MiniProfiler.Current);
            }

            Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
            {
                get { return typeof(ProfiledClientBatchingBatcherFactory); }
            }
        }

        internal class ProfiledSqlDbConnection : ProfiledDbConnection
        {
            public ProfiledSqlDbConnection(SqlConnection connection, MiniProfiler profiler)
                : base(connection, profiler)
            {
                Connection = connection;
            }

            public SqlConnection Connection { get; set; }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                return new ProfiledSqlDbTransaction(Connection.BeginTransaction(isolationLevel), this);
            }
        }

        internal class ProfiledSqlDbTransaction : ProfiledDbTransaction
        {
            public ProfiledSqlDbTransaction(SqlTransaction transaction, ProfiledDbConnection connection)
                : base(transaction, connection)
            {
                Transaction = transaction;
            }

            public SqlTransaction Transaction { get; set; }
        }

        internal class ProfiledSqlDbCommand : ProfiledDbCommand
        {
            public ProfiledSqlDbCommand(SqlCommand cmd, DbConnection conn, MiniProfiler profiler)
                : base(cmd, conn, profiler)
            {
                Command = cmd;
            }

            public SqlCommand Command { get; set; }

            private DbTransaction trans;

            protected override DbTransaction DbTransaction
            {
                get { return trans; }
                set
                {
                    trans = value;
                    var profiledSqlDbTransaction = value as ProfiledSqlDbTransaction;
                    Command.Transaction = profiledSqlDbTransaction == null ? (SqlTransaction)value : profiledSqlDbTransaction.Transaction;
                }
            }
        }

        internal class ProfiledClientBatchingBatcherFactory : IBatcherFactory
        {
            public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
            {
                return new ProfiledSqlClientBatchingBatcher(connectionManager, interceptor);
            }
        }

        internal class ProfiledSqlClientBatchingBatcher : AbstractBatcher
        {
            private int batchSize;
            private int totalExpectedRowsAffected;
            private SqlClientSqlCommandSet currentBatch;
            private StringBuilder currentBatchCommandsLog;
            private readonly int defaultTimeout;

            public ProfiledSqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
                : base(connectionManager, interceptor)
            {
                batchSize = Factory.Settings.AdoBatchSize;
                defaultTimeout = PropertiesHelper.GetInt32(global::NHibernate.Cfg.Environment.CommandTimeout, global::NHibernate.Cfg.Environment.Properties, -1);

                currentBatch = CreateConfiguredBatch();
                //we always create this, because we need to deal with a scenario in which
                //the user change the logging configuration at runtime. Trying to put this
                //behind an if(log.IsDebugEnabled) will cause a null reference exception 
                //at that point.
                currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
            }

            public override int BatchSize
            {
                get { return batchSize; }
                set { batchSize = value; }
            }

            protected override int CountOfStatementsInCurrentBatch
            {
                get { return currentBatch.CountOfCommands; }
            }

            public override void AddToBatch(IExpectation expectation)
            {
                totalExpectedRowsAffected += expectation.ExpectedRowCount;
                var batchUpdate = CurrentCommand;

                string lineWithParameters = null;
                var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
                if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled)
                {
                    lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
                    var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
                    lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
                    currentBatchCommandsLog.Append("command ")
                        .Append(currentBatch.CountOfCommands)
                        .Append(":")
                        .AppendLine(lineWithParameters);
                }
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Adding to batch:" + lineWithParameters);
                }
                currentBatch.Append(((ProfiledSqlDbCommand)batchUpdate).Command);

                if (currentBatch.CountOfCommands >= batchSize)
                {
                    ExecuteBatchWithTiming(batchUpdate);
                }
            }

            protected void ProfiledPrepare(IDbCommand cmd)
            {
                try
                {
                    var sessionConnection = ConnectionManager.GetConnection();

                    cmd.Connection = cmd.Connection != null ? sessionConnection : ((ProfiledSqlDbConnection)sessionConnection).Connection;

                    var trans = (ProfiledSqlDbTransaction)typeof(global::NHibernate.Transaction.AdoTransaction).InvokeMember("trans", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, ConnectionManager.Transaction, null);
                    if (trans != null)
                        cmd.Transaction = trans.Transaction;
                    Factory.ConnectionProvider.Driver.PrepareCommand(cmd);
                }
                catch (InvalidOperationException ioe)
                {
                    throw new ADOException($"While preparing {cmd.CommandText} an error occurred", ioe);
                }
            }

            protected override void DoExecuteBatch(IDbCommand ps)
            {
                Log.DebugFormat("Executing batch");
                CheckReaders();
                ProfiledPrepare(currentBatch.BatchCommand);
                if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
                {
                    Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
                    currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
                }

                int rowsAffected;
                try
                {
                    rowsAffected = currentBatch.ExecuteNonQuery();
                }
                catch (DbException e)
                {
                    throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
                }

                Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

                currentBatch.Dispose();
                totalExpectedRowsAffected = 0;
                currentBatch = CreateConfiguredBatch();
            }

            private SqlClientSqlCommandSet CreateConfiguredBatch()
            {
                var result = new SqlClientSqlCommandSet();
                if (defaultTimeout > 0)
                {
                    try
                    {
                        result.CommandTimeout = defaultTimeout;
                    }
                    catch (Exception e)
                    {
                        if (Log.IsWarnEnabled)
                        {
                            Log.Warn(e.ToString());
                        }
                    }
                }
                return result;
            }
        }

        #endregion
    }
}