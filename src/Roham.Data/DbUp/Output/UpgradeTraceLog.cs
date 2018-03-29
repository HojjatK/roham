using System;
using System.Diagnostics;

namespace Roham.Data.DbUp.Output
{
    internal class UpgradeTraceLog : IUpgradeLog
    {
        public void WriteDebug(string text)
        {
            Trace.WriteLine($"DEBUG:  {text}");
        }

        public void WriteInfo(string text)
        {
            Trace.WriteLine($"INFO:  {text}");
        }

        public void WriteWarning(string text)
        {
            Trace.WriteLine($"WARN:  {text}");
        }

        public void WriteError(string text, Exception exception = null)
        {
            Trace.WriteLine($"ERROR:  {text}{((exception != null) ? exception.ToString() : string.Empty)}");
        }
    }
}
