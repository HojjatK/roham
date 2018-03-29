using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Roham.Data.Providers
{
    public class SqlServerDatabaseProvider : DatabaseProviderBase, IDatabaseProvider, IDatabaseDDLProvider
    {
        public DbProviders Name
        {
            get { return DbProviders.SqlServer; }
        }

        public bool TryConnect(string connectionString, out string errorMessage)
        {
            return CheckDatabaseConnection(GetConnectionFactory(connectionString), out errorMessage);
        }

        public Func<IDbConnection> GetConnectionFactory(string connectionString)
        {
            return () => new SqlConnection(connectionString);
        }

        public string BuildConnectionString(DatabaseInfo dbInfo)
        {
            var sqlConnStrBuilder = new SqlConnectionStringBuilder();
            sqlConnStrBuilder.DataSource = dbInfo.DataSource;
            sqlConnStrBuilder.InitialCatalog = dbInfo.InitialCatalog;
            sqlConnStrBuilder.Pooling = dbInfo.Pooling;

            if (string.IsNullOrWhiteSpace(dbInfo.UserName))
            {
                sqlConnStrBuilder.IntegratedSecurity = true;
            }
            else
            {
                sqlConnStrBuilder.UserID = dbInfo.UserName;
                if (!string.IsNullOrWhiteSpace(dbInfo.Password))
                {
                    sqlConnStrBuilder.Password = dbInfo.Password;
                }
            }
            return sqlConnStrBuilder.ToString();
        }

        public bool IsFullTextEnabled(IDbConnection connection, string tableName)
        {
            string sqlQuery = $@"SELECT convert(int, SERVERPROPERTY('IsFullTextInstalled')) + OBJECTPROPERTY(OBJECT_ID('{tableName}'), 'TableFullTextChangeTrackingOn')";
            var records = new AdhocSqlRunner(() =>
            {
                var dbCommand = CreateDbCommand();
                dbCommand.Connection = connection;
                return dbCommand;
            }).ExecuteReader(sqlQuery);

            bool isEnabled = false;
            var firstRecord = records.FirstOrDefault();
            if (firstRecord != null)
            {
                int i;
                if (int.TryParse(firstRecord.First().Value, out i) && i == 2)
                {
                    isEnabled = true;
                }
            }

            return isEnabled;
        }

        protected IDbCommand CreateDbCommand()
        {
            return new SqlCommand();
        }

        void IDatabaseDDLProvider.CreateDatabase(DatabaseInfo dbInfo)
        {
            if (dbInfo.DbProvider != DbProviders.SqlServer)
            {
                throw new InvalidOperationException("Invalid database provider");
            }
            using (var sqlConnection = GetConnectionFactory(BuildMasterDatabaseConnectionString(dbInfo))())
            {
                sqlConnection.Open();
                var sqlRunner = new AdhocSqlRunner(() => sqlConnection.CreateCommand());
                sqlRunner.ExecuteNonQuery(@"CREATE DATABASE [@database]".Replace("@database", dbInfo.InitialCatalog));
            }
        }

        void IDatabaseDDLProvider.DropDatabase(DatabaseInfo dbInfo)
        {
            using (var sqlConnection = GetConnectionFactory(BuildMasterDatabaseConnectionString(dbInfo))())
            {
                sqlConnection.Open();
                var sqlRunner = new AdhocSqlRunner(() => sqlConnection.CreateCommand());
                sqlRunner.ExecuteNonQuery(@"
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'@database')
BEGIN 
    ALTER DATABASE [@database] 
    SET OFFLINE 
    WITH ROLLBACK IMMEDIATE;

    ALTER DATABASE [@database]
    SET ONLINE;

    DROP DATABASE [@database]; 
END
".Replace("@database", dbInfo.InitialCatalog));
            }
        }

        private string BuildMasterDatabaseConnectionString(DatabaseInfo dbInfo)
        {
            var masterDbInfo = new DatabaseInfo(dbInfo.DbProvider, dbInfo.DataSource, "master", dbInfo.UserName, dbInfo.Password, dbInfo.Pooling);
            return BuildConnectionString(masterDbInfo);
        }
    }
}