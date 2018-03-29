using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Roham.Data.DbUp.Output;
using Roham.Data.DbUp.ScriptProviders;
using Roham.Data.DbUp.ScriptExecutor;

namespace Roham.Data.DbUp
{
    public interface IUpgradeEngineScriptProviderBuilder
    {
        IUpgradeEngineProcessorBuilder WithScriptsFromFileSystem(string scriptsPath);
        IUpgradeEngineProcessorBuilder WithScriptsFromFileSystem(string scriptsPath, Func<string, bool> filter);
        IUpgradeEngineProcessorBuilder WithScriptsFromFileSystem(string scriptsPath, Func<string, bool> filter, Encoding encoding);
        IUpgradeEngineProcessorBuilder WithScriptsEmbeddedInAssembly(Assembly assembly, Func<string, bool> filter);
        IUpgradeEngineProcessorBuilder WithScriptsEmbeddedInAssembly(Assembly assembly, Func<string, bool> filter, Encoding encoding);
    }

    public interface IUpgradeEngineProcessorBuilder
    {
        IUpgradeEngineProcessorBuilder WithPreprocessor(IScriptPreprocessor preprocessor);
        IUpgradeEngineParamsBuilder JournalTo(IUpgradeJournal journal);
        IUpgradeEngineParamsBuilder JournalToTable();
    }

    public interface IUpgradeEngineParamsBuilder
    {
        IUpgradeEngineParamsBuilder WithVariables(IDictionary<string, string> variables);
        IUpgradeEngineParamsBuilder WithDefaultSchema(string schema);
        IUpgradeEngineParamsBuilder ShowSql(bool value);
        IUpgradeEngineParamsBuilder LogScriptOutput(bool value);
        IUpgradeEngineTransactionBuilder WithExecutionTimeout(TimeSpan? timeout);
    }

    public interface IUpgradeEngineTransactionBuilder
    {
        IUpgradeEngineLogBuilder WithoutTransaction();
        IUpgradeEngineLogBuilder WithTransaction();
        IUpgradeEngineLogBuilder WithTransactionPerScript();
    }

    public interface IUpgradeEngineLogBuilder
    {
        IUpgradeEngineBuilderDone LogTo(IUpgradeLog log);
        IUpgradeEngineBuilderDone LogToConsole();
        IUpgradeEngineBuilderDone LogToTrace();
    }

    public interface IUpgradeEngineBuilderDone
    {
        UpgradeEngine Build(IDatabaseProviderFactory dbProviderFactory);
    }

    public class UpgradeEngineBuilder :
        IUpgradeEngineScriptProviderBuilder, IUpgradeEngineProcessorBuilder, IUpgradeEngineParamsBuilder,
        IUpgradeEngineTransactionBuilder, IUpgradeEngineLogBuilder, IUpgradeEngineBuilderDone
    {
        private readonly List<Action<UpgradeConfiguration>> callbacks = new List<Action<UpgradeConfiguration>>();

        public UpgradeEngineBuilder()
        {
        }

        public IUpgradeEngineScriptProviderBuilder WithDatabase(DatabaseInfo dbInfo)
        {
            Configure(c => c.DatabaseInfo = dbInfo);
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineScriptProviderBuilder.WithScriptsFromFileSystem(string scriptsPath)
        {
            Configure(c => c.ScriptProvider = new FileSystemScriptProvider(scriptsPath));
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineScriptProviderBuilder.WithScriptsFromFileSystem(string scriptsPath, Func<string, bool> filter)
        {
            Configure(c => c.ScriptProvider = new FileSystemScriptProvider(scriptsPath, filter));
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineScriptProviderBuilder.WithScriptsFromFileSystem(string scriptsPath, Func<string, bool> filter, Encoding encoding)
        {
            Configure(c => c.ScriptProvider = new FileSystemScriptProvider(scriptsPath, filter, encoding));
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineScriptProviderBuilder.WithScriptsEmbeddedInAssembly(Assembly assembly, Func<string, bool> filter)
        {
            Configure(c => c.ScriptProvider = new EmbeddedScriptProvider(assembly, filter));
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineScriptProviderBuilder.WithScriptsEmbeddedInAssembly(Assembly assembly, Func<string, bool> filter, Encoding encoding)
        {
            Configure(c => c.ScriptProvider = new EmbeddedScriptProvider(assembly, filter, encoding));
            return this;
        }

        IUpgradeEngineProcessorBuilder IUpgradeEngineProcessorBuilder.WithPreprocessor(IScriptPreprocessor preprocessor)
        {
            Configure(c => c.ScriptPreprocessor = preprocessor);
            return this;
        }

        // use a custom journal
        IUpgradeEngineParamsBuilder IUpgradeEngineProcessorBuilder.JournalTo(IUpgradeJournal journal)
        {
            Configure(c => c.UpgradeJournal = journal);
            return this;
        }

        IUpgradeEngineParamsBuilder IUpgradeEngineProcessorBuilder.JournalToTable()
        {
            Configure(c => c.UpgradeJournal = new SqlTableJournal(c));
            return this;
        }

        IUpgradeEngineParamsBuilder IUpgradeEngineParamsBuilder.WithVariables(IDictionary<string, string> variables)
        {
            Configure(c => c.Variables = variables);
            return this;
        }

        IUpgradeEngineParamsBuilder IUpgradeEngineParamsBuilder.WithDefaultSchema(string schema)
        {
            Configure(c => c.DefaultSchema = schema);
            return this;
        }
        IUpgradeEngineParamsBuilder IUpgradeEngineParamsBuilder.ShowSql(bool value)
        {
            Configure(c => c.ShowSql = value);
            return this;
        }
        IUpgradeEngineParamsBuilder IUpgradeEngineParamsBuilder.LogScriptOutput(bool value)
        {
            Configure(c => c.LogScriptOutput = value);
            return this;
        }

        IUpgradeEngineTransactionBuilder IUpgradeEngineParamsBuilder.WithExecutionTimeout(TimeSpan? timeout)
        {
            // TODO:
            return this;
        }

        IUpgradeEngineLogBuilder IUpgradeEngineTransactionBuilder.WithoutTransaction()
        {
            Configure(c => c.TransactionMode = TransactionMode.NoTransaction);
            return this;
        }

        IUpgradeEngineLogBuilder IUpgradeEngineTransactionBuilder.WithTransaction()
        {
            Configure(c => c.TransactionMode = TransactionMode.SingleTransaction);
            return this;
        }

        IUpgradeEngineLogBuilder IUpgradeEngineTransactionBuilder.WithTransactionPerScript()
        {
            Configure(c => c.TransactionMode = TransactionMode.TransactionPerScript);
            return this;
        }

        IUpgradeEngineBuilderDone IUpgradeEngineLogBuilder.LogTo(IUpgradeLog log)
        {
            Configure(c => c.UpgradeLog = log);
            return this;
        }

        IUpgradeEngineBuilderDone IUpgradeEngineLogBuilder.LogToConsole()
        {
            Configure(c => c.UpgradeLog = new UpgradeConsoleLog());
            return this;
        }

        IUpgradeEngineBuilderDone IUpgradeEngineLogBuilder.LogToTrace()
        {
            Configure(c => c.UpgradeLog = new UpgradeTraceLog());
            return this;
        }

        UpgradeEngine IUpgradeEngineBuilderDone.Build(IDatabaseProviderFactory dbProviderFactory)
        {
            UpgradeConfiguration config = new UpgradeConfiguration(dbProviderFactory);
            callbacks.ForEach(c => c(config));
            config.ScriptExecutor = new SqlScriptExecutor(config);
            return new UpgradeEngine(config);
        }

        private void Configure(Action<UpgradeConfiguration> callback)
        {
            callbacks.Add(callback);
        }
    }
}