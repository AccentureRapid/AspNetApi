using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LCMS.Common;
using System.Reflection;

namespace LCMS.Common.Data
{
    public static class DatabaseUtilities
    {
        private const string PARAMETER_PREFIX = "^[@:]|(param_)|(pvar_)|(pnum_)|(pdat_)|(pint_)";
        private static readonly Hashtable _bindVariables = new Hashtable();
        private static readonly Hashtable _matchingProperties = new Hashtable();
        public static List<T> AutoMap<T>(this IDataReader reader, Action<T, IDataRecord> customMapper, int expectedPropertyMatches) where T : new()
        {
            List<T> list = new List<T>();
            List<string> columnNames = reader.getColumnNames();
            List<string> matchedColumns = reader.PopulateListWithMatchingColumns(customMapper, columnNames, list);
            if (matchedColumns.Count != expectedPropertyMatches)
            {
                throw new Exception(string.Format("Expected property matches = {0}, actual = {1}", expectedPropertyMatches, columnNames.Count));
            }
            return list;
        }
        public static List<T> AutoMap<T>(this IDataReader reader)
        {
            System.Type t = typeof(T);
            List<T> result;
            if (t.IsPrimitive || t.Equals(typeof(string)) || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Nullable<>)))
            {
                result = DatabaseUtilities.autoMapSimpleType<T>(reader);
            }
            else
            {
                result = reader.AutoMap<T>(new ColumnNameMapping[0]);
            }
            return result;
        }
        private static List<T> autoMapSimpleType<T>(IDataReader reader)
        {
            if (reader.FieldCount != 1)
            {
                throw new Exception("Database must return 1 column to be able to automap to simple type");
            }
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(Utilities.ConvertTo<T>(reader[0]));
            }
            return list;
        }
        public static List<T> AutoMap<T>(this IDataReader reader, ColumnNameMapping[] customNameMappings)
        {
            List<T> list = new List<T>();
            List<string> columnNames = reader.getColumnNames();
            List<string> matchedColumns = reader.PopulateListWithMatchingColumns(null, columnNames, customNameMappings, list);
            if (matchedColumns.Count != columnNames.Count)
            {
                throw new Exception(string.Format("The following columns could not be mapped - {0}", string.Join(",", columnNames.Except(matchedColumns).ToArray<string>())));
            }
            return list;
        }
        public static object GetDbValue(object value, System.Data.Common.DbCommand command)
        {
            if (!(command is System.Data.SqlClient.SqlCommand) && value != null)
            {
                if (typeof(bool?).IsAssignableFrom(value.GetType()))
                {
                    value = Utilities.ConvertTo<int?>(value);
                }
            }
            return value;
        }
        public static List<string> PopulateListWithMatchingColumns<T>(this IDataReader reader, Action<T, IDataRecord> customMapper, List<string> columnNames, List<T> listToPopulate) where T : new()
        {
            return reader.PopulateListWithMatchingColumns(customMapper, columnNames, null, listToPopulate);
        }
        public static List<string> PopulateListWithMatchingColumns<T>(this IDataReader reader, Action<T, IDataRecord> customMapper, List<string> columnNames, ColumnNameMapping[] customNameMappings, List<T> listToPopulate)
        {
            Dictionary<PropertyInfo, string> propertyMapping = DatabaseUtilities.getPropertyMapping<T>(columnNames, customNameMappings);
            while (reader.Read())
            {
                T temp = System.Activator.CreateInstance<T>();
                foreach (PropertyInfo property in propertyMapping.Keys)
                {
                    string columnName = propertyMapping[property];
                    if (columnName != null)
                    {
                        property.SetValue(temp, Utilities.ConvertTo(property.PropertyType, reader[columnName]), null);
                    }
                }
                if (customMapper != null)
                {
                    customMapper(temp, reader);
                }
                listToPopulate.Add(temp);
            }
            return (
                from p in propertyMapping.Values
                where p != null
                select p).ToList<string>();
        }
        private static Dictionary<PropertyInfo, string> getPropertyMapping<T>(List<string> columnNames, ColumnNameMapping[] customNameMappings)
        {
            Dictionary<PropertyInfo, string> mapping = new Dictionary<PropertyInfo, string>();
            T temp = System.Activator.CreateInstance<T>();
            IEnumerable<PropertyInfo> properties =
               from p in Utilities.GetProperties(temp)
               where p.CanWrite
               select p;
            using (IEnumerator<PropertyInfo> enumerator = properties.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PropertyInfo property = enumerator.Current;
                    string columnName = null;
                    if (customNameMappings != null)
                    {
                        ColumnNameMapping columnNameMapping = customNameMappings.FirstOrDefault((ColumnNameMapping m) => property.Name == m.ColumnName);
                        if (columnNameMapping != null && columnNames.Contains(columnNameMapping.ColumnName))
                        {
                            columnName = columnNameMapping.ColumnName;
                        }
                    }
                    if (columnName == null)
                    {
                        columnName = columnNames.FirstOrDefault((string p) => property.Name.Equals(p.Pascalize(), System.StringComparison.CurrentCultureIgnoreCase));
                    }
                    mapping[property] = columnName;
                }
            }
            return mapping;
        }
        public static List<T> AutoMap<T>(this IDataReader reader, params string[] autoMapColumns) where T : new()
        {
            return reader.AutoMap<T>(null, autoMapColumns);
        }
        public static List<T> AutoMap<T>(this IDataReader reader, Action<T, IDataRecord> customMapper, params string[] autoMapColumns) where T : new()
        {
            if (autoMapColumns == null || autoMapColumns.Length == 0)
            {
                throw new Exception("Empty columns to map");
            }
            List<T> list = new List<T>();
            List<string> columnNames = new List<string>(autoMapColumns);
            List<string> matchedColumns = reader.PopulateListWithMatchingColumns(customMapper, columnNames, list);
            if (matchedColumns.Count != columnNames.Count)
            {
                string[] unmatchedColumns = autoMapColumns.Except(matchedColumns).ToArray<string>();
                throw new Exception(string.Format("Unmatched columns - {0}", string.Join(", ", unmatchedColumns)));
            }
            return list;
        }
        public static List<string> ExtractBindVariables(string sql)
        {
            List<string> result;
            lock (DatabaseUtilities._bindVariables.SyncRoot)
            {
                List<string> parameters = DatabaseUtilities._bindVariables[sql] as List<string>;
                if (parameters == null)
                {
                    parameters = new List<string>();
                    string sqlWithoutGlobals = sql.Replace("@@", string.Empty);
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[@:]\\w+([, )\\r\\n]|$)");
                    System.Text.RegularExpressions.MatchCollection matches = regex.Matches(sqlWithoutGlobals);
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        parameters.Add(System.Text.RegularExpressions.Regex.Replace(match.Value, "[, )\r\n]", string.Empty));
                    }
                    parameters.Remove("@InternalNewId");
                    parameters.Remove(":InternalNewId");
                    DatabaseUtilities._bindVariables[sql] = parameters;
                }
                result = parameters;
            }
            return result;
        }
        public static string RemoveParameterPrefix(string name)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(string.Format("{0}(?<newName>.+)$", "^[@:]|(param_)|(pvar_)|(pnum_)|(pdat_)|(pint_)"), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regex.Replace(name, "${newName}");
        }
        public static string GetMatchingPropertyName(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new System.ArgumentException("parameter cannot be null or empty");
            }
            string result;
            lock (DatabaseUtilities._matchingProperties.SyncRoot)
            {
                if (DatabaseUtilities._matchingProperties["DBParameter" + parameter] == null)
                {
                    DatabaseUtilities._matchingProperties["DBParameter" + parameter] = DatabaseUtilities.RemoveParameterPrefix(parameter).Pascalize();
                }
                result = (string)DatabaseUtilities._matchingProperties["DBParameter" + parameter];
            }
            return result;
        }
        public static void ValidateBindVariables(this System.Data.Common.DbCommand command)
        {
            if (command.CommandType != System.Data.CommandType.Text)
            {
                throw new Exception("Cannot call ValidateBindVariables if not parameterized query");
            }
            List<string> bindVariables = DatabaseUtilities.ExtractBindVariables(command.CommandText);
            System.Data.Common.DbParameterCollection dbParameters = command.Parameters;
            foreach (string variable in bindVariables)
            {
                bool bound = false;
                foreach (System.Data.Common.DbParameter parameter in dbParameters)
                {
                    if (parameter.ParameterName.Equals(variable, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        bound = true;
                        break;
                    }
                }
                if (!bound)
                {
                    throw new Exception(string.Format("{0} is not bound", variable));
                }
            }
        }
        private static List<string> getColumnNames(this IDataReader reader)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                names.Add(reader.GetName(i));
            }
            return names;
        }
        public static string GetInsertStatement(System.Type type)
        {
            Dictionary<string, string> columnNames = DatabaseUtilities.getColumns(type);
            columnNames.Remove(DatabaseUtilities.GetKeyColumn(type).Key);
            return string.Format("INSERT INTO {0}({1}) VALUES({2})", DatabaseUtilities.GetTableName(type), string.Join(",", columnNames.Keys), string.Join(",", columnNames.Values));
        }
        public static string GetUpdateStatement(System.Type type)
        {
            Dictionary<string, string> columnNames = DatabaseUtilities.getColumns(type);
            columnNames.Remove(DatabaseUtilities.GetKeyColumn(type).Key);
            return string.Format("UPDATE {0} SET {1}", DatabaseUtilities.GetTableName(type), string.Join(",",
                from x in columnNames
                select string.Format("{0} = {1}", x.Key, x.Value)));
        }
        public static string GetSelectStatement(System.Type type)
        {
            Dictionary<string, string> columnNames = DatabaseUtilities.getColumns(type);
            return string.Format("SELECT {0} FROM {1}", DatabaseUtilities.buildSelectColumns(columnNames), DatabaseUtilities.GetTableName(type));
        }
        //public static string GetTimestampStatement(System.Type type)
        //{
        //    PropertyInfo timestampProperty = (
        //        from p in type.GetProperties()
        //        where p.GetCustomAttributes(typeof(TimestampAttribute), true).FirstOrDefault<object>() != null
        //        select p).FirstOrDefault<PropertyInfo>();
        //    if (timestampProperty == null)
        //    {
        //        throw new Exception(string.Format("Class {0} does not contain TimestampAttribute", type.Name));
        //    }
        //    ColumnAttribute timestampColumn = (ColumnAttribute)timestampProperty.GetCustomAttributes(typeof(ColumnAttribute), true).First<object>();
        //    KeyValuePair<string, string> key = DatabaseUtilities.GetKeyColumn(type);
        //    return string.Format("SELECT {0} FROM {1} WHERE {2} = @id", timestampColumn.Name, DatabaseUtilities.GetTableName(type), key.Key);
        //}
        private static string buildSelectColumns(Dictionary<string, string> columnNames)
        {
            List<string> columns = new List<string>();
            foreach (KeyValuePair<string, string> column in columnNames)
            {
                if (column.Key == column.Value)
                {
                    columns.Add(column.Key);
                }
                else
                {
                    columns.Add(string.Format("{0} {1}", column.Key, column.Value.Replace("@", string.Empty)));
                }
            }
            return string.Join(", ", columns);
        }
        public static KeyValuePair<string, string> GetKeyColumn(System.Type type)
        {
            PropertyInfo key = (
                from p in type.GetProperties()
                where p.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault<object>() != null
                select p).FirstOrDefault<PropertyInfo>();
            if (key == null)
            {
                throw new Exception("Key not defined in " + type.Name);
            }
            ColumnAttribute colAttribute = (ColumnAttribute)key.GetCustomAttributes(typeof(ColumnAttribute), true).First<object>();
            return new KeyValuePair<string, string>(colAttribute.Name, key.Name);
        }
        private static Dictionary<string, string> getColumns(System.Type type)
        {
            Dictionary<string, string> columnNames = new Dictionary<string, string>();
            IEnumerable<PropertyInfo> properties =
               from p in type.GetProperties()
               where p.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault<object>() != null
               select p;
            if (properties.Count<PropertyInfo>() == 0)
            {
                throw new Exception("Column attributes not found on type " + type.Name);
            }
            columnNames = new Dictionary<string, string>();
            foreach (PropertyInfo p2 in properties)
            {
                ColumnAttribute colAttribute = (ColumnAttribute)p2.GetCustomAttributes(typeof(ColumnAttribute), true).First<object>();
                columnNames[colAttribute.Name] = "@" + p2.Name;
            }
            return columnNames;
        }
        public static string GetTableName(System.Type type)
        {
            TableAttribute attribute = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault<object>();
            if (attribute == null)
            {
                throw new Exception("Table attribute not found on " + type.Name);
            }
            return attribute.Name;
        }
    }
}
