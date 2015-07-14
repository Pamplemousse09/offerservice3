using System;
using log4net;
using log4net.Core;
using System.Diagnostics;
using Kikai.Logging.DTO;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Kikai.Logging.Utils
{
    public class LoggingUtility : ILog
    {
        public enum EntryType
        {
            Info,
            Warning,
            Error
        }

        #region Private fields

        private readonly ILog _log;

        #endregion

        internal LoggingUtility(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        private LoggingUtility(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        #region Implementation of ILoggerWrapper

        ILogger ILoggerWrapper.Logger
        {
            get { return _log.Logger; }
        }

        #endregion

        #region Implementation of ILog

        #region Debug

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Debug(message);
        }

        public void DebugFormat(string format, object arg0)
        {
            DebugFormat(format, new[] { arg0 });
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            var message = string.Format(provider, format, args);
            Debug(message);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Info

        public void Info(object message)
        {
            _log.Info(message);
        }

        /// <summary>
        /// This method prints info messages in JSON format for logging purposes to be parsed in loggly
        /// </summary>
        /// <param name="Object"></param>
        public void InfoJson(LogObject Object)
        {
            string JsonString = JsonConvert.SerializeObject(Object);
            this.Info(JsonString);
        }

        /// <summary>
        /// This method prints error messages in JSON format for logging
        /// </summary>
        /// <param name="Object"></param>
        public void ErrorJson(LogObject Object)
        {
            string JsonString = JsonConvert.SerializeObject(Object);
            this.Error(JsonString);
        }

        /// <summary>
        /// This method prints debug messages in JSON format for logging
        /// </summary>
        /// <param name="Object"></param>
        public void ProcessingDebug(string requestId, string process)
        {
            this.Debug("\"RequestId\":\"" + requestId + "\",\"Process\":\"" + process + "\"");
        }

        /// <summary>
        /// This method inserts monitor info for instrumentation use
        /// </summary>
        /// <param name="Object"></param>
        public void InfoMonitor(MonitorObject Object)
        {
            this.InfoFormat("OfferService|{0}|{1}|{2}|{3}|{4}|{5}", Object.Method, Object.Path, Object.RequestId, Object.Success, Object.StatusCode, Object.ResponseTime);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Info(message);
        }

        public void InfoFormat(string format, object arg0)
        {
            InfoFormat(format, new[] { arg0 });
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            var message = string.Format(provider, format, args);
            Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Warn

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Warn(message);
        }

        public void WarnFormat(string format, object arg0)
        {
            WarnFormat(format, new[] { arg0 });
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            var message = string.Format(provider, format, args);
            Warn(message);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Error

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            bool shouldLogException = true;
            bool shouldLogError = true;
            if (exception is ThreadAbortException)
                shouldLogError = false;
            else if (exception != null && exception.InnerException != null && exception.InnerException is TaskCanceledException)
                shouldLogException = false;
            if (shouldLogError && shouldLogException)
                _log.Error(message, exception);
            else if (shouldLogError)
                _log.Error(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Error(message);
        }

        public void ErrorFormat(string format, object arg0)
        {
            ErrorFormat(format, new[] { arg0 });
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            var message = string.Format(provider, format, args);
            Error(message);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Fatal

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Fatal(message);
        }

        public void FatalFormat(string format, object arg0)
        {
            FatalFormat(format, new[] { arg0 });
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            var message = string.Format(provider, format, args);
            Fatal(message);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
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
        #endregion

        #endregion

        #region Stopwatch
        public void LogElapsed(Stopwatch sw, string actionDescription, object details = null, EntryType entryType = EntryType.Info)
        {
            WriteToLog(entryType,
                "{0}, Elapsed {1}. {2}",
                actionDescription,
                sw.Elapsed,
                details);
        }

        public void LogElapsed(
            Stopwatch sw,
            string actionDescription,
            int thresholdMilliseconds,
            object details = null,
            EntryType entryType = EntryType.Warning)
        {
            LogElapsed(sw, actionDescription, TimeSpan.FromMilliseconds(thresholdMilliseconds), details, entryType);
        }

        public void LogElapsed(
            Stopwatch sw,
            string actionDescription,
            TimeSpan threshold,
            object details = null,
            EntryType entryType = EntryType.Warning)
        {
            var elapsed = sw.Elapsed;
            if (elapsed > threshold)
            {
                WriteToLog(entryType, "{0} -> {1} (Expected {2}). {3}",
                    actionDescription,
                    elapsed,
                    threshold,
                    details);
            }
        }

        protected virtual void WriteToLog(EntryType entryType, string formatString, params object[] args)
        {
            if (entryType == EntryType.Info)
            {
                InfoFormat(formatString, args);
            }
            else if (entryType == EntryType.Warning)
            {
                WarnFormat(formatString, args);
            }
            else if (entryType == EntryType.Error)
            {
                ErrorFormat(formatString, args);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        #endregion


    }
}
