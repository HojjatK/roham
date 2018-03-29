using System;

namespace Roham.Lib.Logger
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        void Debug(string message);
        void Info(string message);
        void Warn(string message, Exception e = null);
        void Error(string message, Exception e = null);
        void Fatal(string message, Exception e = null);
    }
}
