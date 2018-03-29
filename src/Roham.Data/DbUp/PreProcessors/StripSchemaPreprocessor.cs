using System.Text.RegularExpressions;

namespace Roham.Data.DbUp.PreProcessors
{
    public class StripSchemaPreprocessor : IScriptPreprocessor
    { 
        public string Process(string contents)
        {
            return Regex.Replace(contents, @"\$schema\$\.", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
