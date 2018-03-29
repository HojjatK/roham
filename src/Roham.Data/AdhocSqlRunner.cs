using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roham.Data
{
    public sealed class AdhocSqlRunner
    {
        private readonly static Regex _tokenRegex = new Regex(@"\$(?<variableName>\w+)\$");
        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();
        private readonly Func<IDbCommand> _commandFactory;
        private bool _variablesEnabled = false;
        private string _schema = null;

        public AdhocSqlRunner(Func<IDbCommand> commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public AdhocSqlRunner WithSchema(string schema)
        {
            _schema = schema;
            return this;
        }

        public AdhocSqlRunner WithVariable(string variableName, string value)
        {
            _variables.Add(variableName, value);
            _variablesEnabled = true;
            return this;
        }

        public object ExecuteScalar(string query, params Func<string, object>[] parameters)
        {
            object result = null;
            Execute(query, parameters, cmd => result = cmd.ExecuteScalar());
            return result;
        }

        public int ExecuteNonQuery(string query, params Func<string, object>[] parameters)
        {
            int result = 0;
            Execute(query, parameters, cmd => result = cmd.ExecuteNonQuery());
            return result;
        }

        public List<Dictionary<string, string>> ExecuteReader(string query, params Func<string, object>[] parameters)
        {
            var results = new List<Dictionary<string, string>>();
            Execute(query, parameters,
                cmd =>
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var line = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            var value = reader.GetValue(i);
                            value = value == DBNull.Value ? null : value.ToString();
                            line.Add(name, (string)value);
                        }
                        results.Add(line);
                    }
                });
            return results;
        }

        public static string QuoteSqlObjectName(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            objectName = objectName.Trim();

            const int SqlSysnameLength = 128;
            if (objectName.Length > SqlSysnameLength)
            {
                throw new ArgumentOutOfRangeException("objectName", "A SQL server object name is maximum 128 characters long");
            }

            // The ] in the string need to be doubled up so it means we always need an un-even number of ]
            if (objectName.StartsWith("[") && objectName.EndsWith("]") && objectName.Count(x => x == ']') % 2 == 1)
            {
                return objectName;
            }

            return string.Concat("[", objectName.Replace("]", "]]"), "]");
        }

        private void Execute(string commandText, IEnumerable<Func<string, object>> parameters, Action<IDbCommand> executor)
        {
            commandText = Preprocess(commandText);
            using (var cmd = _commandFactory())
            {
                cmd.CommandText = commandText;
                foreach (var param in parameters)
                {
                    var key = param.Method.GetParameters()[0].Name;
                    var value = param(null);
                    var p = cmd.CreateParameter();
                    p.ParameterName = key;
                    p.Value = value;
                    cmd.Parameters.Add(p);
                }
                executor(cmd);
            }
        }

        private string Preprocess(string query)
        {
            if (string.IsNullOrEmpty(_schema))
            {
                query = Regex.Replace(query, @"\$schema\$\.", string.Empty, RegexOptions.IgnoreCase);
            }

            if (!string.IsNullOrEmpty(_schema) && !_variables.ContainsKey("schema"))
            {
                _variables.Add("schema", QuoteSqlObjectName(_schema));
            }

            if (_variablesEnabled)
            {
                query = VariableSubstitutionPreprocessor(_variables, query);
            }

            return query;
        }

        private static string VariableSubstitutionPreprocessor(Dictionary<string, string> variables, string contents)
        {
            return _tokenRegex.Replace(contents, match =>
            {
                var variableName = match.Groups["variableName"].Value;
                if (!variables.ContainsKey(variableName))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, $"Variable {variableName} has no value defined"));
                }
                return variables[variableName];
            });
        }
    }
}
