using System;
using System.Collections.Generic;

namespace Roham.Data.DbUp
{
    public enum TransactionMode
    {
        NoTransaction,
        SingleTransaction,
        TransactionPerScript
    }

    public interface IUpgradeConfiguration
    {
        IDictionary<string, string> Variables { get; }
        DatabaseInfo DatabaseInfo { get; }
        IDatabaseProviderFactory DatabaseProviderFactory { get; }
        IScriptProvider ScriptProvider { get; }
        IScriptPreprocessor ScriptPreprocessor { get; }
        IUpgradeJournal UpgradeJournal { get; }
        IScriptExecutor ScriptExecutor { get; }
        IUpgradeLog UpgradeLog { get; }
        TransactionMode TransactionMode { get; }
        bool ShowSql { get; }
        bool LogScriptOutput { get; }
        string DefaultSchema { get; }
    }

    internal class UpgradeConfiguration : IUpgradeConfiguration
    {
        private IDictionary<string, string> _variables = new Dictionary<string, string>();

        public UpgradeConfiguration(IDatabaseProviderFactory dbProviderFactory)
        {
            DatabaseProviderFactory = dbProviderFactory;
        }
        public IDictionary<string, string> Variables
        {
            get { return _variables; }
            set { _variables = value; }
        }
        public DatabaseInfo DatabaseInfo { get; set; }
        public IDatabaseProviderFactory DatabaseProviderFactory { get; private set; }
        public IScriptProvider ScriptProvider { get; set; }
        public IScriptPreprocessor ScriptPreprocessor { get; set; }
        public IUpgradeJournal UpgradeJournal { get; set; }
        public IScriptExecutor ScriptExecutor { get; set; }
        public IUpgradeLog UpgradeLog { get; set; }
        public TransactionMode TransactionMode { get; set; }
        public bool ShowSql { get; set; }
        public bool LogScriptOutput { get; set; }
        public string DefaultSchema { get; set; }
        public void Validate()
        {
            if (DatabaseInfo == null) throw new NullReferenceException("DatabaseInfo");
            if (ScriptProvider == null) throw new NullReferenceException("ScriptProvider");
        }
    }
}
