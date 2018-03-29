using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Roham.Data.DbUp.ScriptProviders
{
    internal class EmbeddedScriptProvider : IScriptProvider
    {
        private readonly Assembly assembly;
        private readonly Func<string, bool> filter;
        private Encoding encoding;

        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter)
        {
            this.assembly = assembly;
            this.filter = filter;
            this.encoding = Encoding.Default;
        }

        public EmbeddedScriptProvider(Assembly assembly, Func<string, bool> filter, Encoding encoding)
        {
            this.assembly = assembly;
            this.filter = filter;
            this.encoding = encoding;
        }

        public IEnumerable<SqlScript> GetScripts()
        {

            return assembly
                .GetManifestResourceNames()
                .Where(filter)
                .OrderBy(x => x)
                .Select(s => SqlScript.FromStream(s, assembly.GetManifestResourceStream(s), encoding))
                .ToList();
        }
    }
}
