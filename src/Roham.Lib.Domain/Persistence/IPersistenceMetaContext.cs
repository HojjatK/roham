using System.Reflection;

namespace Roham.Lib.Domain.Persistence
{
    public interface IPersistenceMetaContext
    {
        Assembly MappingsAssembly { get; }
        void UpdateSchema(string connectionString);
        void ExportSchemaScript(string connectionString, string filePath);
    }

    public interface IPersistenceMetaContextFactory
    {
        IPersistenceMetaContext CreateMeta();
    }
}
