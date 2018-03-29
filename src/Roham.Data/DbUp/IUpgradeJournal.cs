namespace Roham.Data.DbUp
{
    public interface IUpgradeJournal
    {
        string Name { get; }
        
        string[] GetExecutedScripts();

        void StoreExecutedScript(SqlScript script);
    }
}
