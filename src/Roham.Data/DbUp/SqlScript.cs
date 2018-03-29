using System.IO;
using System.Text;

namespace Roham.Data.DbUp
{
    public class SqlScript
    {
        public SqlScript(string name, string contents)
        {
            Name = name;
            Contents = contents;
        }
        
        public string Name { get; }
        public string Contents { get; }

        public static SqlScript FromFile(string path, Encoding encoding)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var fileName = new FileInfo(path).Name;
                return FromStream(fileName, fileStream, encoding);
            }
        }

        public static SqlScript FromStream(string scriptName, Stream stream, Encoding encoding)
        {
            using (var resourceStreamReader = new StreamReader(stream, encoding, true))
            {
                string c = resourceStreamReader.ReadToEnd();
                return new SqlScript(scriptName, c);
            }
        }
    }
}