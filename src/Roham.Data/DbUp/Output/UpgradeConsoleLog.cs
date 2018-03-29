using System;

namespace Roham.Data.DbUp.Output
{
    internal class UpgradeConsoleLog : IUpgradeLog
    {
        public void WriteInfo(string text)
        {
            Write(ConsoleColor.White, text);
        }

        public void WriteDebug(string text)
        {
            Write(ConsoleColor.Cyan, text);
        }

        public void WriteWarning(string text)
        {
            Write(ConsoleColor.Yellow, text);
        }

        public void WriteError(string text, Exception exception = null)
        {
            Write(ConsoleColor.Red, text);
        }

        public static void Write(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
