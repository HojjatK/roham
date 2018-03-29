using System.Collections.Generic;

namespace Roham.Data.DbUp.PreProcessors
{
    public class DefaultScriptPreProcessor : IScriptPreprocessor
    {
        private readonly StripSchemaPreprocessor stripSchemaPreprocessor;
        private readonly VariableSubstitutionPreprocessor variableSubstitutionPreProcessor;

        public DefaultScriptPreProcessor(IDictionary<string, string> variables)
        {
            stripSchemaPreprocessor = new StripSchemaPreprocessor();
            variableSubstitutionPreProcessor = new VariableSubstitutionPreprocessor(variables);
        }

        public string Process(string contents)
        {
            var newContents = stripSchemaPreprocessor.Process(contents);
            return variableSubstitutionPreProcessor.Process(newContents);
        }
    }
}
