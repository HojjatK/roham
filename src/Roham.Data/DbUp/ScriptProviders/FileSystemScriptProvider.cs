using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Roham.Data.DbUp.ScriptProviders
{
    internal class FileSystemScriptProvider : IScriptProvider
    {
        private readonly string _directoryPath;
        private readonly Func<string, bool> _filter;
        private readonly Encoding _encoding;

        public FileSystemScriptProvider(string directoryPath)
            : this(directoryPath, null)
        { }

        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter) : this(directoryPath, filter, Encoding.Default) { }

        public FileSystemScriptProvider(string directoryPath, Func<string, bool> filter, Encoding encoding)
        {
            _directoryPath = directoryPath;
            _filter = filter;
            _encoding = encoding;
        }

        public IEnumerable<SqlScript> GetScripts()
        {
            var files = Directory.GetFiles(_directoryPath, "*.sql").AsEnumerable();
            if (_filter != null)
            {
                files = files.Where(_filter);
            }
            return files.Select(x => SqlScript.FromFile(x, _encoding)).ToList();
        }
    }
}
