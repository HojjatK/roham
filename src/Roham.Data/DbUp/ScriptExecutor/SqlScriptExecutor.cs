using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Roham.Lib.Logger;

namespace Roham.Data.DbUp.ScriptExecutor
{
    internal class SqlScriptExecutor : IScriptExecutor, IConnectionFactoryHolder
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<SqlScriptExecutor>();
        private readonly UpgradeConfiguration Config;

        public SqlScriptExecutor(UpgradeConfiguration upgradeConfig)
        {
            Config = upgradeConfig;
        }

        public ConnectionFactory ConnectionFactory { get; set; }

        private IDbCommand CreateCommand()
        {
            if (ConnectionFactory == null)
            {
                throw new NullReferenceException("ConnectionManager");
            }
            return ConnectionFactory.GetCommand(ConnectionFactory.GetConnection(), ConnectionFactory.CurrentTransaction);
        }

        public int? ExecutionTimeoutSeconds { get; set; }

        public void Execute(SqlScript script)
        {
            Execute(script, null);
        }

        public void Execute(SqlScript script, IDictionary<string, string> variables)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, string>();
            }

            string infoMsg = $"Executing SQL Server script '{script.Name}'";
            Config.UpgradeLog.WriteInfo(infoMsg);
            Log.Info(infoMsg);

            var contents = Config.ScriptPreprocessor.Process(script.Contents);
            var scriptStatements = SplitScriptIntoCommands(contents).ToList();
            var index = -1;
            try
            {

                scriptStatements
                    .ForEach(statement =>
                    {
                        index++;
                        if (Config.ShowSql)
                        {
                            Config.UpgradeLog.WriteDebug($"{Environment.NewLine}{statement}{Environment.NewLine}");
                        }

                        using (var command = CreateCommand())
                        {
                            command.CommandText = statement;
                            if (ExecutionTimeoutSeconds != null)
                            {
                                command.CommandTimeout = ExecutionTimeoutSeconds.Value;
                            }

                            if (Config.LogScriptOutput)
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    LogReader(reader);
                                }
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    });

            }
            catch (SqlException sqlException)
            {
                Config.UpgradeLog.WriteError($"SQL exception has occured in script: '{script.Name}'");
                Config.UpgradeLog.WriteError($"Script block number: {index}; Block line {sqlException.LineNumber}, Procedure:{sqlException.Procedure}, Number:{sqlException.Number},  Message: {sqlException.Message}");

                Log.Error($"SQL exception has occured in script: '{script.Name}'", sqlException);
                throw;
            }
            catch (DbException sqlException)
            {
                Config.UpgradeLog.WriteError("DB exception has occured in script: '{script.Name}'");
                Config.UpgradeLog.WriteError($"Script block number: {index}; Error code {sqlException.ErrorCode}; Message: {sqlException.Message}");

                Log.Error($"DB exception has occured in script: '{script.Name}'", sqlException);
                throw;
            }
            catch (Exception ex)
            {
                Config.UpgradeLog.WriteError($"Exception has occured in script: '{script.Name}'");
                Config.UpgradeLog.WriteError($"Message: '{ex.Message}'");

                Log.Error($"Exception has occured in script: '{script.Name}'", ex);
                throw;
            }
        }

        public void VerifySchema()
        {
            if (string.IsNullOrEmpty(Config.DefaultSchema))
            {
                return;
            }

            // TODO: consider database provider
            var sqlRunner = new AdhocSqlRunner(() => CreateCommand());
            sqlRunner.ExecuteNonQuery($@"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{Config.DefaultSchema}') Exec('CREATE SCHEMA [{Config.DefaultSchema}]')");
        }

        private IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            using (var reader = CreateCommandReader(scriptContents))
            {
                var commands = new List<string>();
                reader.ReadAllCommands(commands.Add);
                return commands;
            }
        }

        private DbCommandReader CreateCommandReader(string sqlText)
        {
            // TODO: consider database provider
            return new SqlServerCommandReader(sqlText);
        }

        private void LogReader(IDataReader reader)
        {
            do
            {
                var names = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    names.Add(reader.GetName(i));
                }

                if (names.Count == 0)
                    return;

                var lines = new List<List<string>>();
                while (reader.Read())
                {
                    var line = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        value = value == DBNull.Value ? null : value.ToString();
                        line.Add((string)value);
                    }
                    lines.Add(line);
                }

                string format = "";
                int totalLength = 0;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    int maxLength = !lines.Any() ? (names[i].Length + 2) : lines.Max(l => (l[i] ?? "").Length) + 2;
                    format += " {" + i + ", " + maxLength + "} |";
                    totalLength += (maxLength + 3);
                }
                format = "|" + format;
                totalLength += 1;

                Config.UpgradeLog.WriteInfo(new string('-', totalLength));
                Config.UpgradeLog.WriteInfo(String.Format(format, names.ToArray()));
                Config.UpgradeLog.WriteInfo(new string('-', totalLength));
                foreach (var line in lines)
                {
                    Config.UpgradeLog.WriteInfo(String.Format(format, line.ToArray()));
                }
                Config.UpgradeLog.WriteInfo(new string('-', totalLength));
                Config.UpgradeLog.WriteInfo("\r\n");
            } while (reader.NextResult());
        }
    }
}
