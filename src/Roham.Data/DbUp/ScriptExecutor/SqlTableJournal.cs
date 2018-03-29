using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Roham.Lib.Logger;

namespace Roham.Data.DbUp.ScriptExecutor
{
    internal interface IConnectionFactoryHolder
    {
        ConnectionFactory ConnectionFactory { get; set; }
    }

    internal class SqlTableJournal : IUpgradeJournal, IConnectionFactoryHolder
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<SqlScriptExecutor>();
        private readonly UpgradeConfiguration Config;

        public SqlTableJournal(UpgradeConfiguration upgradeConfig)
        {
            Config = upgradeConfig;
        }

        public string Name => "[__MigrationHistory]";

        public string SchemaTableName => string.IsNullOrEmpty(Config.DefaultSchema)
            ? AdhocSqlRunner.QuoteSqlObjectName(Name)
            : AdhocSqlRunner.QuoteSqlObjectName(Config.DefaultSchema) + "." + AdhocSqlRunner.QuoteSqlObjectName(Name);

        public ConnectionFactory ConnectionFactory { get; set; }

        private IDbCommand CreateCommand()
        {
            if (ConnectionFactory == null)
            {
                throw new NullReferenceException("ConnectionManager");
            }
            return ConnectionFactory.GetCommand(ConnectionFactory.GetConnection(), ConnectionFactory.CurrentTransaction);
        }

        public string[] GetExecutedScripts()
        {
            Config.UpgradeLog.WriteInfo("Fetching list of already executed scripts.");

            if (!DoesTableExist())
            {
                Config.UpgradeLog.WriteInfo($"The {SchemaTableName} table could not be found. The database is assumed to be at version 0.");
                return new string[0];
            }

            var scripts = new List<string>();
            using (var command = CreateCommand())
            {
                command.CommandText = $@"select [ScriptName] from {SchemaTableName} order by [ScriptName]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        scripts.Add((string)reader[0]);
                    }
                }
            }
            return scripts.ToArray();
        }

        public void StoreExecutedScript(SqlScript script)
        {
            if (!DoesTableExist())
            {
                Config.UpgradeLog.WriteInfo($"Creating the {SchemaTableName} table");

                using (var command = CreateCommand())
                {
                    command.CommandText =
$@"create table {SchemaTableName} (
	[Id] int identity(1,1) not null constraint PK_SchemaVersions_Id primary key,
	[ScriptName] nvarchar(255) not null,
	[Applied] datetime not null
)";
                    command.ExecuteNonQuery();
                    Config.UpgradeLog.WriteInfo($"The {SchemaTableName} table has been created");
                }
            }

            using (var command = CreateCommand())
            {
                command.CommandText = $@"insert into {SchemaTableName} (ScriptName, Applied) values (@scriptName, @applied)";
                var scriptNameParam = command.CreateParameter();
                scriptNameParam.ParameterName = "scriptName";
                scriptNameParam.Value = script.Name;
                command.Parameters.Add(scriptNameParam);

                var appliedParam = command.CreateParameter();
                appliedParam.ParameterName = "applied";
                appliedParam.Value = DateTime.Now;
                command.Parameters.Add(appliedParam);

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        private bool DoesTableExist()
        {
            using (var command = CreateCommand())
            {
                command.CommandText = $@"select count(*) from {SchemaTableName}";
                try
                {
                    command.ExecuteScalar();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
                catch (DbException)
                {
                    return false;
                }
            }
        }
    }
}