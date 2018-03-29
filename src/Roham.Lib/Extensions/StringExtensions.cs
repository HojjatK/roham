using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string Repeat(this string source, int times)
        {
            if (times <= 0) return "";

            var sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                sb.Append(source);
            }
            return sb.ToString();
        }

        public static string Slugify(this string source)
        {
            string value = source ?? string.Empty;
            value = value.ToLower();
            value = Regex.Replace(value, "\\s", "-");
            value = new string(value.Select(x => (char.IsLetterOrDigit(x) || x == '-' || x == '/') ? x : '-').ToArray());
            value = Regex.Replace(value, "-+", "-");
            value = value.Trim('-', '/');
            return value;
        }
    }
}
