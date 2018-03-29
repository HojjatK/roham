using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Roham.Data
{
    public enum DbProviders
    {
        None,
        SqlServer,
        SQLite,
    }

    public sealed class DatabaseInfo
    {
        public DatabaseInfo(
            DbProviders provider,
            string dataSource,
            string initialCatalog,
            string userName,
            string password,
            bool pooling)
        {
            switch (provider)
            {
                case DbProviders.SqlServer:
                    Init(
                        provider,
                        dataSource,
                        initialCatalog,
                        userName,
                        password,
                        pooling, true, true, true);
                    break;
                case DbProviders.SQLite:
                    Init(
                        provider,
                        dataSource,
                        initialCatalog,
                        userName,
                        password,
                        pooling, false, false, false);
                    break;
                default:
                    throw new NotSupportedException($"{provider} is not supported.");
            }
        }

        public DatabaseInfo(
            DbProviders provider,
            string connectionString)
        {
            switch (provider)
            {
                case DbProviders.SqlServer:
                    var sqlConnStrBuilder = new SqlConnectionStringBuilder(connectionString);
                    Init(
                        provider,
                        sqlConnStrBuilder.DataSource,
                        sqlConnStrBuilder.InitialCatalog,
                        sqlConnStrBuilder.IntegratedSecurity ? null : sqlConnStrBuilder.UserID,
                        sqlConnStrBuilder.IntegratedSecurity ? null : sqlConnStrBuilder.Password,
                        sqlConnStrBuilder.Pooling, true, true, true);
                    break;
                case DbProviders.SQLite:
                    var sqliteConnStrBuilder = new SQLiteConnectionStringBuilder(connectionString);
                    Init(
                        provider,
                        sqliteConnStrBuilder.DataSource,
                        null,
                        null,
                        sqliteConnStrBuilder.Password,
                        sqliteConnStrBuilder.Pooling, false, false, false);
                    break;
                default:
                    throw new NotSupportedException($"{provider} is not supported.");
            }

        }

        private void Init(
            DbProviders provider,
            string dataSource,
            string initialCatalog,
            string userName,
            string password,
            bool pooling,
            bool supportSchema,
            bool supportFullText,
            bool supportFuture)
        {
            DbProvider = provider;
            DataSource = ReplaceDataDirectory(dataSource);
            InitialCatalog = initialCatalog;
            UserName = userName;
            Password = password;
            Pooling = pooling;
            SupportSchema = supportSchema;
            SupportFullText = supportFullText;
            SupportFuture = supportFuture;
        }

        public DbProviders DbProvider { get; private set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; private set; }
        public bool SupportSchema { get; private set; }
        public bool SupportFuture { get; private set; }
        public bool SupportFullText { get; private set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Pooling { get; set; }

        public bool Validate(out string errorMessage)
        {
            var sb = new StringBuilder();
            switch (DbProvider)
            {
                case DbProviders.SqlServer:
                    if (string.IsNullOrWhiteSpace(DataSource))
                    {
                        sb.AppendLine("Database server is empty");
                    }
                    if (string.IsNullOrWhiteSpace(InitialCatalog))
                    {
                        sb.AppendLine("Database name is empty");
                    }

                    if (string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    {
                        sb.AppendLine("database username needs to be specified");
                    }
                    if (!string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Password))
                    {
                        sb.AppendLine("database password needs to be specified");
                    }
                    break;
                case DbProviders.SQLite:
                    if (string.IsNullOrWhiteSpace(DataSource))
                    {
                        sb.AppendLine("DataSource is empty");
                    }
                    break;
                default:
                    throw new NotSupportedException($"{DbProvider} is not supported");
            }

            errorMessage = sb.ToString();
            return string.IsNullOrEmpty(errorMessage);
        }

        public static string ReplaceDataDirectory(string inputString)
        {
            var str = inputString.Trim();
            if (string.IsNullOrEmpty(inputString) || !inputString.StartsWith("|DataDirectory|", StringComparison.InvariantCultureIgnoreCase))
            {
                return str;
            }
            var data = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(data))
            {
                data = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (string.IsNullOrEmpty(data))
            {
                data = string.Empty;
            }
            var length = "|DataDirectory|".Length;
            if ((inputString.Length > "|DataDirectory|".Length) && ('\\' == inputString["|DataDirectory|".Length]))
            {
                length++;
            }
            return Path.Combine(data, inputString.Substring(length));
        }

        public override string ToString()
        {
            return $@"[{DbProvider}]-[{DataSource}\{InitialCatalog}]";
        }
    }
}
