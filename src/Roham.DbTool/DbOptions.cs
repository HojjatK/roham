using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NDesk.Options;
using Roham.Data;

namespace Roham.DbTool
{
    [Flags]
    public enum DbOptionCommands
    {
        None = 0,
        Create = 1,
        Drop = 2,
        ExportSchema = 4,
        Upgrade = 8,
    }

    public class DbOptions
    {
        private DbOptions(
            DatabaseInfo dbInfo,
            DbOptionCommands dbCommand,
            string exportFileName,
            string upgradePath)
        {
            DatabaseInfo = dbInfo;
            Command = dbCommand;
            ExportScriptFileName = exportFileName;
            UpgradeScriptsPath = upgradePath;
            if (dbInfo != null)
            {
                ConnectionString = new DatabaseProviderFactory().Create(dbInfo.DbProvider).BuildConnectionString(dbInfo);
            }
        }

        public DatabaseInfo DatabaseInfo { get;  }
        public string ConnectionString { get; }
        public DbOptionCommands Command { get; }
        public string ExportScriptFileName { get; }
        public string UpgradeScriptsPath { get; }
        public bool ShowHelp { get; set; }
        public bool ShowSql { get; set; }
        public bool IsForce { get; set; }
        public bool IsDebugMode { get; set; }

        public bool IsValid()
        {
            try
            {
                Validate();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsValid(out string errorMessage)
        {
            if (DatabaseInfo == null)
            {
                errorMessage = "database information is null";
                return false;
            }

            return DatabaseInfo.Validate(out errorMessage);
        }

        public void Validate()
        {
            if (DatabaseInfo == null)
            {
                throw new NullReferenceException("dbInfo");
            }
            string errMsg;
            if (!DatabaseInfo.Validate(out errMsg))
            {
                throw new ArgumentException(errMsg);
            }
            if (DbOptionCommands.ExportSchema == Command && string.IsNullOrWhiteSpace(ExportScriptFileName))
            {
                throw new ArgumentException("ExportScriptFileName");
            }
            if (DbOptionCommands.Upgrade == Command && string.IsNullOrWhiteSpace(UpgradeScriptsPath))
            {
                throw new ArgumentException("UpgradeScriptsPath");
            }
        }

        public override string ToString()
        {
            string db = "";
            if (DatabaseInfo != null)
            {
                db = string.Format("{0}{1}", DatabaseInfo.DataSource, !string.IsNullOrWhiteSpace(DatabaseInfo.InitialCatalog) ? @"\" + DatabaseInfo.InitialCatalog : "");
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Database:      {0}\r\n", db);
            sb.AppendFormat("Action(s):\r\n{0}", ToString(Command));
            if (!string.IsNullOrEmpty(DatabaseInfo != null ? DatabaseInfo.UserName : ""))
            {
                sb.AppendFormat("Username:       {0}\r\n", DatabaseInfo.UserName);
            }
            if (!string.IsNullOrEmpty(ExportScriptFileName))
            {
                sb.AppendFormat("ExportScript:   {0}\r\n", ExportScriptFileName);
            }
            if (!string.IsNullOrEmpty(UpgradeScriptsPath))
            {
                sb.AppendFormat("UpgradeScript:  {0}\r\n", UpgradeScriptsPath);
            }
            var options = new List<string> { IsDebugMode ? "DebugMode" : "", ShowSql ? "ShowSql" : "", IsForce ? "ForceMode" : "" }
                .Where(s => !string.IsNullOrEmpty(s)).ToList();
            var optionStr = string.Join(", ", options);
            if (!string.IsNullOrEmpty(optionStr))
            {
                sb.AppendFormat("Options:       {0}\r\n", optionStr);
            }
            return sb.ToString();
        }

        private string ToString(DbOptionCommands command)
        {
            var sb = new StringBuilder();
            if ((command & DbOptionCommands.Drop) == DbOptionCommands.Drop)
            {
                sb.AppendLine("  Drop database");
            }
            if ((command & DbOptionCommands.Create) == DbOptionCommands.Create)
            {
                sb.AppendLine("  Create database");
            }
            if ((command & DbOptionCommands.ExportSchema) == DbOptionCommands.ExportSchema)
            {
                sb.AppendLine("  Export database schema");
            }
            if ((command & DbOptionCommands.Upgrade) == DbOptionCommands.Upgrade)
            {
                sb.AppendLine("  Upgrade database");
            }
            return sb.ToString();
        }

        public static bool TryParse(string[] args, out DbOptions options)
        {
            options = null;
            try
            {
                options = Parse(args);
                return true;
            }
            catch (FormatException formatExp)
            {
                ConsoleLog.WriteInfo(formatExp.Message);
                return false;
            }
            catch (Exception e)
            {
                ConsoleLog.WriteError($"An unexpected error happened: {e.Message}");
                return false;
            }
        }

        public static DbOptions Parse(string[] args)
        {
            bool showHelp = false, showSql = false, isForce = false, isDebugMode = false;
            DbProviders providerName = DbProviders.SqlServer;
            string dataSource = "", initialCatalog = "", username = "", password = "";
            string exportfilename = "", upgradepath = "";
            var command = DbOptionCommands.None;

            showHelp = false;
            var optionSet = new OptionSet
            {
                { "d", "Drop existing database", _ => command |= DbOptionCommands.Drop },
                { "c", "Create database schema", _ => command |= DbOptionCommands.Create },
                { "e", "Export database schema", _ => command |= DbOptionCommands.ExportSchema },
                { "u", "Upgrade database", _ => command |= DbOptionCommands.Upgrade },
                { "upd=", "Upgrade database with SQL update files, {directory} containing SQL update files", dir => upgradepath = dir },
                { "exf=", "Export database schema {file path}", exf => exportfilename = exf },
                { "p=", "The database {provider} [sqlserver | sqlite. If not specified the default is sqlserver.", p => { if(p == null) providerName = DbProviders.SqlServer; else Enum.TryParse<DbProviders>(p, out providerName); } },
                { "source=", "The database {source} or server host", s => dataSource = s },
                { "catalog=", "The database {catalog} name", d => initialCatalog = d },
                { "user=", "Database {user} identity", u => username = u },
                { "password=", "Database user {password}", p => password = p },

                { "f", "Forces command to run without getting confirmation from user", _ => isForce = true },
                { "debug", "Execute DbTool in debug mode", _ => isDebugMode = true},
                { "showsql", "Show executed sql commands during dbtool execution", _ => showSql = true},
                { "?|h|help",  "Show this message and exit", v => showHelp = v != null },
            };

            optionSet.Parse(args);
            if (args.Length == 0)
            {
                showHelp = true;
            }

            DbOptions o = new DbOptions(
                new DatabaseInfo(providerName, dataSource, initialCatalog, username, password, false),
                command,
                exportfilename,
                upgradepath)
            {
                ShowHelp = showHelp,
                ShowSql = showSql,
                IsForce = isForce,
                IsDebugMode = isDebugMode
            };

            if (showHelp)
            {
                ConsoleLog.WriteSuccess("Roham DbTool Console Application Usage\r\n");
                ConsoleLog.Write(ConsoleColor.White, "DbTool.exe [options]");
                ConsoleLog.WriteInfo("");
                optionSet.WriteOptionDescriptions(Console.Out);
                ConsoleLog.Write(ConsoleColor.White, "\r\nExamples:");
                ConsoleLog.WriteInfo(@"
DbTool -c --source=localhost --catalog=roham-dev --user=sa --password=test
Creates a new sqlserver database named roham-dev in localhost server 

DbTool -c --source=localhost --catalog=roham-dev -e --exf=c:\database\schema.sql
Creates a new sqlserver database named roham-dev and export database 
schema script into c:\database\schema.sql 

DbTool -u --upd=c:\upgrade\scripts --source=localhost --catalog=roham
Runs upgrade scripts located at c:\upgrade\scripts in roham sqlserver 
database at localhost (via IntegratedSecurity)

DbTool -d -c -u --upd:c:\upgrade\scripts --source=localhost --catalog=roham --user=sa --password=test
Drops roham sqlserver database from localhost server and creates a new one 
with same name and then upgrade database with scripts located at c:\upgrade\scripts

DbTool -p=sqlite -c -u --upd=c:\upgrade\scripts --source=c:\database\roham.sqlite --debug --showsql
Creates a new sqlite database at c:\database\roham.sqlite and then runs upgrade scripts 
located at c:\upgrade\scripts in debug mode and show executed sql commands
");
                return o;
            }

            Validate(o);

            return o;
        }

        private static void Validate(DbOptions o)
        {
            var sb = new StringBuilder();

            string errors = "";
            if (((o.Command & DbOptionCommands.Drop) == DbOptionCommands.Drop) ||
                ((o.Command & DbOptionCommands.Create) == DbOptionCommands.Create) ||
                ((o.Command & DbOptionCommands.Upgrade) == DbOptionCommands.Upgrade))
            {
                if (!o.DatabaseInfo.Validate(out errors))
                {
                    sb.Append(errors);
                }
            }

            if (((o.Command & DbOptionCommands.Drop) == DbOptionCommands.Drop) &&
                 ((o.Command & DbOptionCommands.Upgrade) == DbOptionCommands.Upgrade))
            {
                errors += @"database can not be dropped and upgraded at the same time";
            }

            if (((o.Command & DbOptionCommands.Drop) == DbOptionCommands.Drop) &&
                 ((o.Command & DbOptionCommands.ExportSchema) == DbOptionCommands.ExportSchema))
            {
                errors += @"database can not be dropped and schema then export the schema script";
            }

            if (!string.IsNullOrWhiteSpace(o.UpgradeScriptsPath))
            {
                if (!Directory.Exists(o.UpgradeScriptsPath))
                {
                    sb.AppendFormat("SQL update directory {0} not found.", o.UpgradeScriptsPath);
                    sb.AppendLine();
                }
                else if (Directory.GetFiles(o.UpgradeScriptsPath, "*.sql").Length == 0)
                {
                    sb.AppendFormat("SQL update directory {0} does not have any sql file to update.", o.UpgradeScriptsPath);
                    sb.AppendLine();
                }
            }

            string allErrors = sb.ToString();
            if (allErrors != null && !string.IsNullOrEmpty(allErrors))
            {
                throw new FormatException(allErrors);
            }
        }
    }
}