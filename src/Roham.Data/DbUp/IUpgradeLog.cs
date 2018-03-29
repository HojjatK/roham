using System;

namespace Roham.Data.DbUp
{
    public interface IUpgradeLog
    {
        void WriteDebug(string text);
        void WriteInfo(string text);
        void WriteWarning(string text);
        void WriteError(string text, Exception exception = null);
    }
}
