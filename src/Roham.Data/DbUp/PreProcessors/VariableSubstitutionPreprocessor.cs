using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Roham.Data.DbUp.PreProcessors
{
    public class VariableSubstitutionPreprocessor : IScriptPreprocessor
    {
        private readonly IDictionary<string, string> _variables;
        private static readonly Regex tokenRegex = new Regex(@"\$(?<variableName>\w+)\$");
       
        public VariableSubstitutionPreprocessor(IDictionary<string, string> variables)
        {
            _variables = variables;
        }

        public string Process(string contents)
        {
            return tokenRegex.Replace(contents, match => ReplaceToken(match, _variables));
        }

        private static string ReplaceToken(Match match, IDictionary<string, string> variables)
        {
            var variableName = match.Groups["variableName"].Value;
            if (!variables.ContainsKey(variableName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, $"Variable {variableName} has no value defined"));
            }
            return variables[variableName];
        }
    }
}
