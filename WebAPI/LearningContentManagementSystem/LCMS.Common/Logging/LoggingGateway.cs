using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCMS.Common;

namespace LCMS.Common.Logging
{
	public class LoggingGateway : ILoggingGateway
	{
		static LoggingGateway()
		{
			Logger.SetLogWriter(new LogWriterFactory().Create());
		}

		private enum LogEntryTypes
		{
			Warning,
			Error,
			Trace,
			Audit
		}
		public virtual void Audit(string title, string message)
		{
			this.Audit(title, message, System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		public virtual void Audit(string title, string message, string userId)
		{
			this.createLog(title, message, userId, LoggingGateway.LogEntryTypes.Audit);
		}
		public virtual void Trace(string title, string message)
		{
			this.Trace(title, message, System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		public virtual void Trace(System.Exception ex)
		{
			this.Trace(ex, string.Empty);
		}
		public virtual void Trace(System.Exception ex, string additionalMessage)
		{
			string userIdentity = System.Threading.Thread.CurrentPrincipal.Identity.Name;
			string message = LoggingGateway.getFormattedMessageFromException(ex, userIdentity, additionalMessage);
			this.Trace(LoggingGateway.getParsedExceptionType(ex.GetType()), message, userIdentity);
		}
		public virtual void Trace(string title, string message, string userId)
		{
			this.createLog(title, message, userId, LoggingGateway.LogEntryTypes.Trace);
		}
		public virtual void Error(string title, string message)
		{
			this.Error(title, message, System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		public virtual void Error(System.Exception ex)
		{
			this.Error(ex, string.Empty);
		}
		public virtual void Error(System.Exception ex, System.Data.Common.DbCommand command)
		{
			try
			{
				string userIdentity = System.Threading.Thread.CurrentPrincipal.Identity.Name;
				string message = LoggingGateway.getFormattedMessageFromException(ex, userIdentity, string.Empty);
				message += command.GetLogMessage();
				message = message + "Call stack: " + System.Environment.StackTrace;
				this.Error(LoggingGateway.getParsedExceptionType(ex.GetType()), message, userIdentity);
			}
			catch (System.Exception ex2)
			{
				this.Trace(ex2);
			}
		}
		public virtual void Error(System.Exception ex, string additionalMessage)
		{
			string userIdentity = System.Threading.Thread.CurrentPrincipal.Identity.Name;
			string message = LoggingGateway.getFormattedMessageFromException(ex, userIdentity, additionalMessage);
			this.Error(LoggingGateway.getParsedExceptionType(ex.GetType()), message, userIdentity);
		}
		public virtual void Error(string title, string message, string userId)
		{
			this.createLog(title, message, userId, LoggingGateway.LogEntryTypes.Error);
		}
		public virtual void Warning(System.Exception ex)
		{
			this.Warning(ex, string.Empty);
		}
		public virtual void Warning(System.Exception ex, string additionalMessage)
		{
			string userIdentity = System.Threading.Thread.CurrentPrincipal.Identity.Name;
			string message = LoggingGateway.getFormattedMessageFromException(ex, userIdentity, additionalMessage);
			this.Warning(LoggingGateway.getParsedExceptionType(ex.GetType()), message, userIdentity);
		}
		private static string getParsedExceptionType(System.Type exception)
		{
			string[] parsedName = exception.ToString().Split(new char[]
			{
				'.'
			});
			string result;
			if (parsedName.Length != 0)
			{
				result = parsedName[parsedName.Length - 1];
			}
			else
			{
				result = exception.ToString();
			}
			return result;
		}
		public virtual void Warning(string title, string message)
		{
			this.Warning(title, message, System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		public virtual void Warning(string title, string message, string userId)
		{
			this.createLog(title, message, userId, LoggingGateway.LogEntryTypes.Warning);
		}
		private static string getFormattedMessageFromException(System.Exception ex, string userIdentity, string additionalMessage)
		{
			System.Text.StringBuilder message = new System.Text.StringBuilder(ex.Message);
			message.AppendFormat("\nAdditional Message: {0} \n", additionalMessage);
			message.AppendFormat("User Identity: {0} \n", userIdentity);
			message.AppendFormat("Source: {0} \n", ex.Source);
			message.AppendFormat("Target Site: {0} \n", ex.TargetSite);
			message.AppendFormat("Stack Trace: {0} \n", ex.StackTrace);
			message.Append(ex.GetInnerExceptionDetail());
			return message.ToString();
		}
		private void createLog(string title, string message, string userId, LoggingGateway.LogEntryTypes type)
		{
			RapidLogEntry log = new RapidLogEntry();
			log.Title = title;
			log.Message = message;
			log.UserId = userId;

			switch (type)
			{
				case LoggingGateway.LogEntryTypes.Warning:
					log.Severity = System.Diagnostics.TraceEventType.Warning;
					break;
				case LoggingGateway.LogEntryTypes.Error:
					log.Severity = System.Diagnostics.TraceEventType.Error;
					break;
				case LoggingGateway.LogEntryTypes.Trace:
					log.Severity = System.Diagnostics.TraceEventType.Verbose;
					break;
				case LoggingGateway.LogEntryTypes.Audit:
					log.Severity = System.Diagnostics.TraceEventType.Information;
					break;
			}

			log.Categories.Add(type.ToString());

			if (Logger.IsLoggingEnabled())
			{
				Logger.Write(log);
			}
		}
	}
}
