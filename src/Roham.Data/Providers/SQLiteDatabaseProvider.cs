using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Roham.Data.Providers
{
    public class SQLiteDatabaseProvider : DatabaseProviderBase, IDatabaseProvider, IDatabaseDDLProvider
    {
        public DbProviders Name
        {
            get { return DbProviders.SQLite; }
        }

        public bool TryConnect(string connectionString, out string errorMessage)
        {
            return CheckDatabaseConnection(GetConnectionFactory(connectionString), out errorMessage);           
        }

        public Func<IDbConnection> GetConnectionFactory(string connectionString)
        {
            return () => new SQLiteConnection(connectionString);
        }

        public string BuildConnectionString(DatabaseInfo dbInfo)
        {
            var sqliteConnStrBuilder = new SQLiteConnectionStringBuilder();
            sqliteConnStrBuilder.DataSource = DatabaseInfo.ReplaceDataDirectory(dbInfo.InitialCatalog);
            sqliteConnStrBuilder.Version = 3;
            sqliteConnStrBuilder.Pooling = dbInfo.Pooling;
            sqliteConnStrBuilder.JournalMode = SQLiteJournalModeEnum.Wal;
            sqliteConnStrBuilder.DefaultTimeout = 500;

            if (!string.IsNullOrWhiteSpace(dbInfo.Password))
            {
                sqliteConnStrBuilder.Password = dbInfo.Password;
            }

            return sqliteConnStrBuilder.ConnectionString;
        }

        public bool IsFullTextEnabled(IDbConnection connection, string tableName)
        {
            return false;
        }

        protected IDbCommand CreateDbCommand()
        {
            return new SQLiteCommand();
        }

        void IDatabaseDDLProvider.CreateDatabase(DatabaseInfo dbInfo)
        {
            var databaseFileName = DatabaseInfo.ReplaceDataDirectory(dbInfo.InitialCatalog);
            if (!File.Exists(databaseFileName))
            {
                var directoryName = Path.GetDirectoryName(databaseFileName);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                SQLiteConnection.CreateFile(databaseFileName);
            }
            else
            {
                throw new InvalidOperationException($"Database file {databaseFileName} already exists.");
            }
        }

        void IDatabaseDDLProvider.DropDatabase(DatabaseInfo dbInfo)
        {
            SQLiteConnection.ClearAllPools();
            var databaseFileName = DatabaseInfo.ReplaceDataDirectory(dbInfo.InitialCatalog);
            if (File.Exists(databaseFileName))
            {
                File.Delete(databaseFileName);
            }
        }        
    }
}
