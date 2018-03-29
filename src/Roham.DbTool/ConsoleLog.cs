using System;

namespace Roham.DbTool
{
    public class ConsoleLog
    {
        public static void WriteInfo(string info)
        {
            WriteLine(ConsoleColor.Gray, info);
        }

        public static void WriteWarn(string warning)
        {
            WriteLine(ConsoleColor.DarkYellow, warning);
        }

        public static void WriteError(string error)
        {
            WriteLine(ConsoleColor.Red, error);
        }

        public static void WriteSuccess(string success)
        {
            WriteLine(ConsoleColor.Green, success);
        }

        public static void WriteLine(ConsoleColor color, string message)
        {
            Write(color, string.Format("{0}{1}", message, Environment.NewLine));
        }

        public static void Write(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
