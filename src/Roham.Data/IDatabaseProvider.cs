using System;
using System.Data;

namespace Roham.Data
{
    public interface IDatabaseProvider
    {
        DbProviders Name { get; }
        bool TryConnect(string connectionString, out string errorMessage);
        Func<IDbConnection> GetConnectionFactory(string connectionString);        
        string BuildConnectionString(DatabaseInfo dbInfo);
        bool IsFullTextEnabled(IDbConnection connection, string tableName);
    }

    public interface IDatabaseDDLProvider
    {
        void CreateDatabase(DatabaseInfo dbInfo);
        void DropDatabase(DatabaseInfo dbInfo);
    }
}
