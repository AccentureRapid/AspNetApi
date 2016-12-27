using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common.Logging
{
    public interface ILoggingGateway
    {
        void Audit(string title, string message);
        void Audit(string title, string message, string userId);
        void Error(System.Exception ex);
        void Error(System.Exception ex, System.Data.Common.DbCommand command);
        void Error(System.Exception ex, string additionalMessage);
        void Error(string title, string message);
        void Error(string title, string message, string userId);
        void Trace(System.Exception ex);
        void Trace(System.Exception ex, string additionalMessage);
        void Trace(string title, string message);
        void Trace(string title, string message, string userId);
        void Warning(System.Exception ex);
        void Warning(System.Exception ex, string additionalMessage);
        void Warning(string title, string message);
        void Warning(string title, string message, string userId);
    }
}
