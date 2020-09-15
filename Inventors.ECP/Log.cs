using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public static class Log
    {
        public static void Debug(string format, params object[] args)
        {
            if (Level == LogLevel.DEBUG)
            {
                AddToLogger(LogLevel.DEBUG, string.Format(CultureInfo.CurrentCulture, format, args));
            }
        }

        public static void Status(string format, params object[] args)
        {
            if (Level <= LogLevel.STATUS)
            {
                AddToLogger(LogLevel.STATUS, string.Format(CultureInfo.CurrentCulture, format, args));
            }
        }

        public static void Error(string format, params object[] args)
        {
            if (Level <= LogLevel.ERROR)
            {
                AddToLogger(LogLevel.ERROR, string.Format(CultureInfo.CurrentCulture, format, args));
            }
        }

        public static void SetLogger(ILogger newLogger)
        {
            logger = newLogger;
        }

        public static LogLevel Level { get; set; } = LogLevel.DEBUG;

        private static void AddToLogger(LogLevel level, string str)
        {
            if (logger != null)
            {
                logger.Add(DateTime.Now, level, str);
            }
        }

        private static ILogger logger;
    }
}
