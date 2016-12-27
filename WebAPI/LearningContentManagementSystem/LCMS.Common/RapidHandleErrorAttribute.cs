using LCMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LCMS.Common
{
    public class RapidHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                System.Exception ex = filterContext.Exception;
                TempDataDictionary tempdata = filterContext.Controller.TempData;
                if (ex is System.Security.SecurityException)
                {
                    tempdata["Error"] = "Access denied. Please contact IT or your manager if you need access.";
                }
                if (RapidHandleErrorAttribute.sessionHasExpired(filterContext))
                {
                    tempdata["Error"] = "Your session has expired.";
                }
                if (tempdata["Error"] != null)
                {
                    base.OnException(filterContext);
                    return;
                }
                RapidLogger.Error(ex, "Error in page " + filterContext.HttpContext.Request.RawUrl);
                tempdata["Error"] = "An unexpected system error has occurred. ";
                TempDataDictionary tempDataDictionary;
                (tempDataDictionary = tempdata)["Error"] = tempDataDictionary["Error"] + "The error has been logged. Please try again or contact the Help Desk if the problem persists.";
                if (ex is HttpRequestValidationException && ex.Message.StartsWith("A potentially dangerous"))
                {
                    tempdata["Error"] = "The application has deemed one of your entries as potentially dangerous. Please review your entires and make sure they do not contain '<' character.";
                }
                if (ex.Message.Contains("timed out"))
                {
                    tempdata["Error"] = "Your request has timed out. Please try again later or contact the Help Desk if the problem persists.";
                }
            }
            base.OnException(filterContext);
        }
        private static bool sessionHasExpired(ExceptionContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            bool result;
            if (context.Session != null && context.Session.IsNewSession)
            {
                string cookieHeader = context.Request.Headers["Cookie"];
                if (cookieHeader != null && cookieHeader.IndexOf("ASP.NET_SessionId") >= 0)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
    }
}
