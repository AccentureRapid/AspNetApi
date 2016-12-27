using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common
{
    public static class Utilities
    {
        private static System.Collections.Hashtable _typeProperties = new System.Collections.Hashtable();
        private static object _syncTypeProperties = new object();
        private static System.Collections.Hashtable _pascalizedStrings = new System.Collections.Hashtable();
        private static object _syncPascalizedStrings = new object();
        public static object nvl(object o1, object o2)
        {
            return (System.Convert.IsDBNull(o1) || o1 == null) ? o2 : o1;
        }
        public static string JoinNonEmpties(this string first, string separator, params string[] parts)
        {
            string str = first;
            for (int i = 0; i < parts.Length; i++)
            {
                string next = parts[i];
                str = (string.IsNullOrEmpty(str) ? (string.IsNullOrEmpty(next) ? string.Empty : next) : (string.IsNullOrEmpty(next) ? str : (str + separator + next)));
            }
            return str;
        }
        public static T ConvertTo<T>(object source)
        {
            return (T)((object)Utilities.ConvertTo(typeof(T), source));
        }
        public static object ConvertTo(System.Type t, object source)
        {
            object temp = System.Convert.IsDBNull(source) ? null : source;
            object result;
            if (temp == null && t.IsValueType)
            {
                result = System.Activator.CreateInstance(t);
            }
            else
            {
                result = Utilities.changeType(t, temp);
            }
            return result;
        }
        public static bool TryConvertTo(System.Type t, object source, out object destination)
        {
            bool result;
            try
            {
                destination = Utilities.ConvertTo(t, source);
                result = true;
            }
            catch (System.Exception ex)
            {
                string temp = source as string;
                if (Utilities.IsNullableType(t) && temp != null && string.IsNullOrEmpty(temp))
                {
                    destination = null;
                    result = true;
                }
                else
                {
                    destination = (t.IsValueType ? System.Activator.CreateInstance(t) : null);
                    result = false;
                }
            }
            return result;
        }
        public static bool IsNullableType(System.Type type)
        {
            bool result;
            if (type.IsValueType)
            {
                if (!type.IsGenericType && typeof(System.Nullable<>).IsAssignableFrom(type))
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }
        private static object changeType(System.Type t, object value)
        {
            object result;
            if ((t == typeof(bool) || t == typeof(bool?)) && value != null && value.GetType() == typeof(string))
            {
                string stringValue = (string)value;
                string text = stringValue.ToUpper();
                switch (text)
                {
                    case "T":
                        result = true;
                        return result;
                    case "F":
                        result = false;
                        return result;
                    case "Y":
                        result = true;
                        return result;
                    case "N":
                        result = false;
                        return result;
                    case "YES":
                        result = true;
                        return result;
                    case "NO":
                        result = false;
                        return result;
                    case "1":
                        result = true;
                        return result;
                    case "0":
                        result = false;
                        return result;
                }
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Nullable<>))
            {
                if (value == null)
                {
                    result = null;
                }
                else
                {
                    result = System.Convert.ChangeType(value, System.Nullable.GetUnderlyingType(t));
                }
            }
            else
            {
                result = System.Convert.ChangeType(value, t);
            }

            return result;
        }
        public static T ConvertTo<T>(object source, T defaultValue)
        {
            return (T)((object)Utilities.ConvertTo(typeof(T), source, defaultValue));
        }
        public static object ConvertTo(System.Type t, object source, object defaultValue)
        {
            object temp = System.Convert.IsDBNull(source) ? null : source;
            return (temp == null) ? defaultValue : Utilities.changeType(t, temp);
        }
        public static string Pascalize(this string name)
        {
            string pascalizedString = (string)Utilities._pascalizedStrings[name];
            string result;
            if (!string.IsNullOrEmpty(pascalizedString))
            {
                result = pascalizedString;
            }
            else
            {
                if (name.Contains('_'))
                {
                    string[] tokens = name.Split(new char[]
					{
						'_'
					});
                    string[] array = tokens;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string token = array[i];
                        pascalizedString = pascalizedString + token[0].ToString().ToUpper() + token.Substring(1).ToLower();
                    }
                }
                else
                {
                    pascalizedString = name[0].ToString().ToUpper() + name.Substring(1);
                }
                lock (Utilities._syncPascalizedStrings)
                {
                    Utilities._pascalizedStrings[name] = pascalizedString;
                }
                result = pascalizedString;
            }
            return result;
        }
        public static string GetFileExtension(this string fileName)
        {
            int indexOfLastDot = fileName.LastIndexOf('.');
            return (indexOfLastDot == -1) ? string.Empty : fileName.Substring(indexOfLastDot);
        }
        public static bool AllPropertiesAreEqualTo<T>(this T source, T target)
        {
            return (
                from p in typeof(T).GetProperties()
                where p.CanRead
                select p).All((System.Reflection.PropertyInfo p) => Utilities.ConvertTo<string>(p.GetValue(source, null)) == Utilities.ConvertTo<string>(p.GetValue(target, null)));
        }
        public static int MonthInterval(System.DateTime from, System.DateTime to)
        {
            return (to.Year - from.Year) * 12 + to.Month - from.Month;
        }
        public static System.Reflection.PropertyInfo[] GetProperties(object model)
        {
            System.Reflection.PropertyInfo[] result;
            lock (Utilities._syncTypeProperties)
            {
                string key = string.Format("{0}_Properties", model.GetType().FullName);
                if (Utilities._typeProperties[key] == null)
                {
                    Utilities._typeProperties[key] = model.GetType().GetProperties();
                }
                result = (Utilities._typeProperties[key] as System.Reflection.PropertyInfo[]);
            }
            return result;
        }
        public static string FormatExcelCsvLine(params object[] fields)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            if (fields != null)
            {
                bool firstField = true;
                for (int i = 0; i < fields.Length; i++)
                {
                    object field = fields[i];
                    string fieldString = ((field == null) ? string.Empty : field.ToString()) ?? string.Empty;
                    if (fieldString.Contains(',') || fieldString.Contains('\r') || fieldString.Contains('\n'))
                    {
                        fieldString = fieldString.Replace("\r\n", "\n");
                        fieldString = fieldString.Replace('\r', '\n');
                        fieldString = fieldString.Replace("\"", "\"\"");
                        fieldString = string.Format("\"{0}\"", fieldString);
                    }
                    if (firstField)
                    {
                        firstField = false;
                    }
                    else
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.Append(fieldString);
                }
            }
            stringBuilder.Append(System.Environment.NewLine);
            return stringBuilder.ToString();
        }
        public static string GetLogMessage(this System.Data.Common.DbCommand command)
        {
            string message = string.Empty;
            if (command != null)
            {
                message = message + "\nSQL: " + command.CommandText;
                System.Text.StringBuilder argumentList = new System.Text.StringBuilder();
                foreach (System.Data.Common.DbParameter parameter in command.Parameters)
                {
                    argumentList.Append(string.Format("{0}={1};", parameter.ParameterName, parameter.Value));
                }
                message = message + "\nParameters: " + argumentList;
            }
            return message;
        }
        public static void RetryOnFailure(Action method, int numberOfRetries = 1, int waitInterval = 10, System.Collections.Generic.List<System.Type> exceptions = null)
        {
            Utilities.RetryOnFailure<object>(() => Utilities.convertToFunc(method), numberOfRetries, waitInterval, exceptions);
        }
        public static T RetryOnFailure<T>(Func<T> method, int numberOfRetries = 1, int waitInterval = 10, System.Collections.Generic.List<System.Type> exceptions = null)
        {
            if (method == null)
            {
                throw new System.ArgumentNullException("method");
            }
            T value = default(T);
            for (int i = 0; i < 1 + numberOfRetries; i++)
            {
                try
                {
                    value = method();
                    break;
                }
                catch (System.Exception ex)
                {
                    if (exceptions != null && !exceptions.Contains(ex.GetType()))
                    {
                        throw;
                    }
                    if (i == numberOfRetries)
                    {
                        throw;
                    }
                    System.Threading.Thread.Sleep(waitInterval * 1000);
                }
            }
            return value;
        }
        public static string GetInnerExceptionDetail(this System.Exception ex)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            for (System.Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                message.AppendFormat("\nInner Exception Message: {0} \n", innerException.Message);
                message.AppendFormat("Inner Exception Stack Trace: {0} \n", innerException.StackTrace);
            }
            return message.ToString();
        }
        public static string Truncate(this string value, int maxLength)
        {
            if (maxLength < 0)
            {
                throw new System.ArgumentOutOfRangeException("maxLength", maxLength, "Must be non-negative");
            }
            string result;
            if (string.IsNullOrEmpty(value))
            {
                result = value;
            }
            else
            {
                result = ((value.Length <= maxLength) ? value : value.Substring(0, maxLength));
            }
            return result;
        }
        private static object convertToFunc(Action method)
        {
            method();
            return null;
        }
    }
}
