using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kikai.Logging;
using Kikai.Logging.Utils.IUtils;
using System.Threading;

namespace Kikai.Logging.Utils
{
    public class LoggerFactory
    {
        // The default logger for the class is "AppLogger". An application can overwrite the default by calling SetDefaultLogger()
        // Any thread can overwrite the logger by calling SetLogger to set the logger for that thread.
        private static ThreadLocal<string> loggerName = new ThreadLocal<string>(() => "AppLogger");
        public static void SetThreadLogger(string logger)
        {
            loggerName.Value = logger;
        }

        public static LoggingUtility GetLogger()
        {
            return new LoggingUtility(loggerName.Value);
        }
        public static LoggingUtility GetLogger(string name)
        {
            return new LoggingUtility(name);
        }
    }
}
