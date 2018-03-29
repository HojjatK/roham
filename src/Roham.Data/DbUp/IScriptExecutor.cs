using System.Collections.Generic;

namespace Roham.Data.DbUp
{
    public interface IScriptExecutor
    {
        int? ExecutionTimeoutSeconds { get; set; }

        void Execute(SqlScript script);
        
        void Execute(SqlScript script, IDictionary<string, string> variables);
        
        void VerifySchema();
    }
}
