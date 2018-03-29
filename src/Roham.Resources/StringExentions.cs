namespace Roham.Resources
{
    public static class StringExentions
    {
        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }
    }
}
