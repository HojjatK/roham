using System;
using Roham.Data.Providers;

namespace Roham.Data
{
    public interface IDatabaseProviderFactory
    {
        IDatabaseProvider Create(DbProviders provider);
    }

    public class DatabaseProviderFactory : IDatabaseProviderFactory
    {
        public IDatabaseProvider Create(DbProviders provider)
        {
            switch (provider)
            {
                case DbProviders.SqlServer:
                    return new SqlServerDatabaseProvider();
                case DbProviders.SQLite:
                    return new SQLiteDatabaseProvider();
                default:
                    throw new NotSupportedException($"{provider} is not supported.");
            }
        }
    }
}
