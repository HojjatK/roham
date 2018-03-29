using System.Collections.Generic;

namespace Roham.Data.DbUp.ScriptProviders
{
    internal sealed class StaticScriptProvider : IScriptProvider
    {
        private readonly IEnumerable<SqlScript> scripts;

        public StaticScriptProvider(IEnumerable<SqlScript> scripts)
        {
            this.scripts = scripts;
        }

        public IEnumerable<SqlScript> GetScripts()
        {
            return scripts;
        }
    }
}