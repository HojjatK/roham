using log4net.Appender;
using System;
using System.Collections.Generic;

namespace Roham.Lib.Logger
{
    public static class LoggerFactory
    {
        public static bool Configure()
        {
            var col = log4net.Config.XmlConfigurator.Configure();
            return col == null || col.Count == 0;
        }

        public static void ChangeLogThresholdToDebug()
        {
            var hierarchy = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
            if (hierarchy == null)
            {
                return;
            }

            hierarchy.GetAppenders().ForEach(appender =>
            {
                var appSkel = appender as AppenderSkeleton;
                if (appSkel != null) appSkel.Threshold = log4net.Core.Level.Debug;
            });
        }

        public static ILogger GetLogger<T>()
        {
            return new LoggerAdapter(log4net.LogManager.GetLogger(typeof(T).FullName));
        }

        public static ILogger GetLogger(string loggerName)
        {
            return new LoggerAdapter(log4net.LogManager.GetLogger(loggerName));
        }

        #region Nested Classes

        private class LoggerAdapter : ILogger
        {
            private readonly log4net.ILog _log;

            public LoggerAdapter(log4net.ILog log)
            {
                _log = log;

            }

            public bool IsDebugEnabled
            {
                get { return _log.IsDebugEnabled; }
            }

            public bool IsInfoEnabled
            {
                get { return _log.IsInfoEnabled; }
            }

            public bool IsWarnEnabled
            {
                get { return _log.IsWarnEnabled; }
            }

            public bool IsErrorEnabled
            {
                get { return _log.IsErrorEnabled; }
            }

            public bool IsFatalEnabled
            {
                get { return _log.IsFatalEnabled; }
            }

            public void Debug(string message)
            {
                _log.Debug(message);
            }

            public void Info(string message)
            {
                _log.Info(message);
            }

            public void Warn(string message, Exception e = null)
            {
                if (e == null)
                    _log.Warn(message);
                else
                    _log.Warn(e, e);
            }

            public void Error(string message, Exception e = null)
            {
                if (e == null)
                    _log.Error(message);
                else
                    _log.Error(message, e);
            }

            public void Fatal(string message, Exception e = null)
            {
                if (e == null)
                    _log.Fatal(message);
                else
                    _log.Fatal(message, e);
            }
        }

        #endregion
    }
}
