using System;
using System.Collections.Generic;

namespace Roham.Data.DbUp.Output
{
    public class UpgradeStringLog : IUpgradeLog
    {
        private readonly List<string> lines = new List<string>();

        public void WriteDebug(string text)
        {
            lines.Add($"DEBUG:  {text}");
        }

        public void WriteInfo(string text)
        {
            lines.Add($"INFO: {text}");
        }

        public void WriteWarning(string text)
        {
            lines.Add($"WARN: {text}");
        }

        public void WriteError(string text, Exception exception = null)
        {
            lines.Add($"ERROR: {text}");
            if (exception != null)
            {
                lines.Add($"Exception:\r\n{exception.ToString()}");
            }
        }

        public List<string> GetOutput()
        {
            return lines;
        }
    }
}
