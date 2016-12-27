using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common.Logging
{
    public static class RapidLogger
    {
        private static ILoggingGateway _loggingGateway;
        public static ILoggingGateway LoggingGateway
        {
            get
            {
                if (RapidLogger._loggingGateway == null)
                {
                    RapidLogger._loggingGateway = new LoggingGateway();
                }
                return RapidLogger._loggingGateway;
            }
        }
        public static void Audit(string title, string message)
        {
            RapidLogger.LoggingGateway.Audit(title, message);
        }
        public static void Audit(string title, string message, string userId)
        {
            RapidLogger.LoggingGateway.Audit(title, message, userId);
        }
        public static void Trace(string title, string message)
        {
            RapidLogger.LoggingGateway.Trace(title, message);
        }
        public static void Trace(System.Exception ex)
        {
            RapidLogger.LoggingGateway.Trace(ex);
        }
        public static void Trace(System.Exception ex, string additionalMessage)
        {
            RapidLogger.LoggingGateway.Trace(ex, additionalMessage);
        }
        public static void Trace(string title, string message, string userId)
        {
            RapidLogger.LoggingGateway.Trace(title, message, userId);
        }
        public static void Error(string title, string message)
        {
            RapidLogger.LoggingGateway.Error(title, message);
        }
        public static void Error(System.Exception ex)
        {
            RapidLogger.LoggingGateway.Error(ex);
        }
        public static void Error(System.Exception ex, System.Data.Common.DbCommand command)
        {
            RapidLogger.LoggingGateway.Error(ex, command);
        }
        public static void Error(System.Exception ex, string additionalMessage)
        {
            RapidLogger.LoggingGateway.Error(ex, additionalMessage);
        }
        public static void Error(string title, string message, string userId)
        {
            RapidLogger.LoggingGateway.Error(title, message, userId);
        }
        public static void Warning(System.Exception ex)
        {
            RapidLogger.LoggingGateway.Warning(ex);
        }
        public static void Warning(System.Exception ex, string additionalMessage)
        {
            RapidLogger.LoggingGateway.Warning(ex, additionalMessage);
        }
        public static void Warning(string title, string message)
        {
            RapidLogger.LoggingGateway.Warning(title, message);
        }
        public static void Warning(string title, string message, string userId)
        {
            RapidLogger.LoggingGateway.Warning(title, message, userId);
        }
    }
}
