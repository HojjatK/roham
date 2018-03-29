using Roham.Data;

namespace Roham.Lib.Domain.Persistence
{
    public interface IPersistenceConfigs
    {
        DbProviders DatabaseProvider { get; }
        string ConnectionString { get; }
        int AdoNetBatchSize { get; }
        bool ShowSql { get; }
    }
}
