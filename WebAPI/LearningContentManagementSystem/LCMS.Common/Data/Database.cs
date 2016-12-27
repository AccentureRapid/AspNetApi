using Microsoft.Practices.EnterpriseLibrary.Data;
using LCMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common.Data
{
    public class Database : IDatabase
    {
        private Microsoft.Practices.EnterpriseLibrary.Data.Database _enterpriseDb;
        private string _connectionString;
        public Database()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            _enterpriseDb = factory.CreateDefault();
            _connectionString = _enterpriseDb.ConnectionString;
        }
        public Database(string name)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            _enterpriseDb = factory.Create(name);
            _connectionString = _enterpriseDb.ConnectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public void AddInParameter(DbCommand command, string name, System.Data.DbType dbType, object value)
        {
            if (command.Parameters.Contains(name))
            {
                command.Parameters.Remove(command.Parameters[name]);
            }

            this._enterpriseDb.AddInParameter(command, name, dbType, value);
        }
        public void AddOutParameter(DbCommand command, string name, System.Data.DbType dbType, int size)
        {
            if (command.Parameters.Contains(name))
            {
                command.Parameters.Remove(command.Parameters[name]);
            }
            this._enterpriseDb.AddOutParameter(command, name, dbType, size);
        }
        public int ExecuteNonQueryCommand(DbCommand command)
        {
            return this.decoratedDatabaseCall<int>(new Func<DbCommand, int>(this._enterpriseDb.ExecuteNonQuery), command);
        }
        public object ExecuteScalarCommand(DbCommand command)
        {
            return this.decoratedDatabaseCall<object>(new Func<DbCommand, object>(this._enterpriseDb.ExecuteScalar), command);
        }
        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            return this._enterpriseDb.GetStoredProcCommand(storedProcedureName);
        }
        public DbCommand GetSqlStringCommand(string sqlStatement)
        {
            return this._enterpriseDb.GetSqlStringCommand(sqlStatement);
        }
        public DbDataAdapter GetDataAdapter()
        {
            return this._enterpriseDb.GetDataAdapter();
        }
        public DbCommandBuilder CreateCommandBuilder()
        {
            return this._enterpriseDb.DbProviderFactory.CreateCommandBuilder();
        }
        public IDataReader ExecuteReader(DbCommand command)
        {
            return this.decoratedDatabaseCall<IDataReader>(new Func<DbCommand, IDataReader>(this._enterpriseDb.ExecuteReader), command);
        }
        public List<T> ExecuteAndReturn<T>(DbCommand command)
        {
            List<T> result;
            using (System.Data.DataTableReader reader = this.ExecuteDataSet(command).CreateDataReader())
            {
                result = reader.AutoMap<T>();
            }
            return result;
        }
        public List<T> ExecuteAndReturn<T>(DbCommand command, params ColumnNameMapping[] customNameMappings) where T : new()
        {
            List<T> result;
            using (System.Data.DataTableReader reader = this.ExecuteDataSet(command).CreateDataReader())
            {
                result = reader.AutoMap<T>(customNameMappings);
            }
            return result;
        }
        public List<T> ExecuteAndReturn<T>(DbCommand command, Action<T, System.Data.IDataRecord> customMapper, int expectedPropertyMatches) where T : new()
        {
            List<T> result;
            using (System.Data.DataTableReader reader = this.ExecuteDataSet(command).CreateDataReader())
            {
                result = reader.AutoMap(customMapper, expectedPropertyMatches);
            }
            return result;
        }
        public List<T> ExecuteAndReturn<T>(DbCommand command, params string[] autoMapColumns) where T : new()
        {
            List<T> result;
            using (System.Data.DataTableReader reader = this.ExecuteDataSet(command).CreateDataReader())
            {
                result = reader.AutoMap<T>(autoMapColumns);
            }
            return result;
        }
        public List<T> ExecuteAndReturn<T>(DbCommand command, Action<T, System.Data.IDataRecord> customMapper, params string[] autoMapColumns) where T : new()
        {
            List<T> result;
            using (System.Data.DataTableReader reader = this.ExecuteDataSet(command).CreateDataReader())
            {
                result = reader.AutoMap(customMapper, autoMapColumns);
            }
            return result;
        }
        public T ExecuteDataSet<T>(DbCommand command) where T : System.Data.DataSet, new()
        {
            System.Data.DataSet resultSet = this.ExecuteDataSet(command);
            T typedDataset = System.Activator.CreateInstance<T>();
            resultSet.Tables[0].TableName = typedDataset.Tables[0].TableName;
            typedDataset.Merge(resultSet.Tables[0]);
            return typedDataset;
        }
        public System.Data.DataSet ExecuteDataSet(DbCommand command)
        {
            return this.decoratedDatabaseCall<System.Data.DataSet>(new Func<DbCommand, System.Data.DataSet>(this._enterpriseDb.ExecuteDataSet), command);
        }
        //public System.Data.DataSet ExecuteDataSet(DbCommand command, string returnCursor)
        //{
        //    this.AddOutCursorParameter(command, returnCursor);
        //    return this.ExecuteDataSet(command);
        //}
        //public void AddInClobParameter(DbCommand command, string name, object value, int size)
        //{
        //    command.AddOracleLobParameter(name, value);
        //}
        //public void AddInBlobParameter(DbCommand command, string name, object value, int size)
        //{
        //    command.AddOracleLobParameter(name, value);
        //}
        //public void AddOutCursorParameter(DbCommand command, string returnCursor)
        //{
        //    command.AddOutCursorParameter(returnCursor);
        //}
        public void AddParametersFrom<T>(DbCommand command, T input, params string[] parametersToAdd)
        {
            System.Collections.Generic.List<string> addedParameters = this.addInParametersFrom<T>(command, input, parametersToAdd);
            if (addedParameters.Count != parametersToAdd.Length)
            {
                string[] unmatchedParameters = parametersToAdd.Except(addedParameters).ToArray<string>();
                throw new Exception(string.Format("Unmatched parameters - {0}", string.Join(", ", unmatchedParameters)));
            }
        }
        public void AddParametersFrom<T>(DbCommand command, T input)
        {
            if (command.CommandType != System.Data.CommandType.Text)
            {
                throw new Exception("The feature is only supported for inline SQL");
            }
            string[] parametersToAdd = DatabaseUtilities.ExtractBindVariables(command.CommandText).ToArray();
            this.AddParametersFrom<T>(command, input, parametersToAdd);
        }
        public int Insert(object obj)
        {
            System.Type type = obj.GetType();
            DbCommand command = this.getCommand(type, new Func<System.Type, string>(DatabaseUtilities.GetInsertStatement));
            string returnSql = command.CommandText;
            string newId = "InternalNewId";
            if (command is System.Data.SqlClient.SqlCommand)
            {
                returnSql = "; SET @InternalNewId = SCOPE_IDENTITY()";
                newId = "@" + newId;
            }
            else
            {
                returnSql = string.Format("RETURNING {0} INTO :InternalNewId", DatabaseUtilities.GetKeyColumn(type).Key);
                newId = ":" + newId;
            }
            DbCommand expr_7A = command;
            expr_7A.CommandText += returnSql;
            this.AddParametersFrom<object>(command, obj);
            this.AddOutParameter(command, newId, System.Data.DbType.Int32, 10);
            this.ExecuteNonQueryCommand(command);
            return (int)command.Parameters[newId].Value;
        }
        public int Update(object obj)
        {
            System.Type type = obj.GetType();
            System.Collections.Generic.KeyValuePair<string, string> key = DatabaseUtilities.GetKeyColumn(type);
            
            DbCommand command = this.getCommand(type, new Func<System.Type, string>(DatabaseUtilities.GetUpdateStatement));
            string whereClause = string.Format(" WHERE {0} = {1}", key.Key, (command is System.Data.SqlClient.SqlCommand) ? ("@" + key.Value) : (":" + key.Value));
            DbCommand expr_6B = command;
            expr_6B.CommandText += whereClause;
            this.AddParametersFrom<object>(command, obj);
            return this.ExecuteNonQueryCommand(command);
        }
        public T Get<T>(int id)
        {
            DbCommand command = this.getCommand(typeof(T), new Func<System.Type, string>(DatabaseUtilities.GetSelectStatement));
            string whereClause = string.Format(" WHERE {0} = {1}", DatabaseUtilities.GetKeyColumn(typeof(T)).Key, (command is System.Data.SqlClient.SqlCommand) ? "@id" : ":id");
            DbCommand expr_56 = command;
            expr_56.CommandText += whereClause;
            this.AddInParameter(command, (command is System.Data.SqlClient.SqlCommand) ? "@id" : ":id", System.Data.DbType.Int64, id);
            return this.ExecuteAndReturn<T>(command).FirstOrDefault<T>();
        }
        public List<T> GetAll<T>()
        {
            System.Type type = typeof(T);
            DbCommand command = this.getCommand(type, new Func<System.Type, string>(DatabaseUtilities.GetSelectStatement));
            return this.ExecuteAndReturn<T>(command);
        }
        public int Delete<T>(int id)
        {
            System.Type type = typeof(T);
            DbCommand command = this.GetSqlStringCommand("DELETE FROM " + DatabaseUtilities.GetTableName(type));
            System.Collections.Generic.KeyValuePair<string, string> key = DatabaseUtilities.GetKeyColumn(type);
            string prefix = (command is System.Data.SqlClient.SqlCommand) ? "@" : ":";
            string whereClause = string.Format(" WHERE {0} = {1}{2}", key.Key, prefix, key.Value);
            DbCommand expr_5C = command;
            expr_5C.CommandText += whereClause;
            this.AddInParameter(command, prefix + key.Value, System.Data.DbType.Int32, id);
            return this.ExecuteNonQueryCommand(command);
        }
        public int UpdateWithConcurrencyCheck(DbCommand updateCommand, string timestampColumn, System.DateTime? originalTimestamp)
        {
            string sql = updateCommand.CommandText;
            string prefix = (updateCommand is System.Data.SqlClient.SqlCommand) ? "@" : ":";
            if (!originalTimestamp.HasValue)
            {
                sql += string.Format(" AND {0} IS NULL", timestampColumn);
            }
            else
            {
                sql += string.Format(" AND {0} = {1}originalTimestamp", timestampColumn, prefix);
                this.AddInParameter(updateCommand, prefix + "originalTimestamp", System.Data.DbType.DateTime, originalTimestamp.Value);
            }
            updateCommand.CommandText = sql;
            int rowsAffected = this.ExecuteNonQueryCommand(updateCommand);
            if (rowsAffected == 0)
            {
                throw new Exception("Concurrency Exception");
            }
            return rowsAffected;
        }
        public void AddInParameter(DbCommand command, string name, System.Data.DbType dbType, string value, bool convertDataType)
        {
            object convertedValue = value;
            if (convertDataType)
            {
                convertedValue = Database.ConvertDataType(dbType, value);
            }
            this.AddInParameter(command, name, dbType, convertedValue);
        }
        public static object ConvertDataType(System.Data.DbType dbType, string value)
        {
            object result;
            if (string.IsNullOrEmpty(value))
            {
                result = null;
            }
            else
            {
                object convertedValue;
                switch (dbType)
                {
                    case System.Data.DbType.DateTime:
                        convertedValue = System.Convert.ToDateTime(value);
                        goto IL_AA;
                    case System.Data.DbType.Decimal:
                        convertedValue = System.Convert.ToDecimal(value);
                        goto IL_AA;
                    case System.Data.DbType.Double:
                        convertedValue = System.Convert.ToDouble(value);
                        goto IL_AA;
                    case System.Data.DbType.Int16:
                        convertedValue = System.Convert.ToInt16(value);
                        goto IL_AA;
                    case System.Data.DbType.Int32:
                        convertedValue = System.Convert.ToInt32(value);
                        goto IL_AA;
                    case System.Data.DbType.Int64:
                        convertedValue = System.Convert.ToInt64(value);
                        goto IL_AA;
                }
                throw new System.NotImplementedException(string.Format("Conversion from {0} is not implemented yet. Please implement the conversion in the framework.", dbType));
            IL_AA:
                result = convertedValue;
            }
            return result;
        }
        private System.Collections.Generic.List<string> addInParametersFrom<T>(DbCommand command, T input, params string[] parametersToAdd)
        {
            if (parametersToAdd == null || parametersToAdd.Length == 0)
            {
                throw new Exception("parametersToAdd is empty");
            }
            System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> properties =
                from p in Utilities.GetProperties(input)
                where p.CanRead
                select p;
            System.Collections.Generic.List<string> addedParameters = new System.Collections.Generic.List<string>();
            System.Array.ForEach<string>(parametersToAdd, delegate(string parameter)
            {
                System.Reflection.PropertyInfo property = properties.FirstOrDefault((System.Reflection.PropertyInfo p) => p.Name.Equals(DatabaseUtilities.GetMatchingPropertyName(parameter), System.StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    this.addInParameterFrom<T>(command, input, parameter, property);
                    addedParameters.Add(parameter);
                }
            });
            return addedParameters;
        }
        private DbCommand getCommand(System.Type type, Func<System.Type, string> sqlBuilder)
        {
            string sql = sqlBuilder(type);
            DbCommand command = this.GetSqlStringCommand(sql);
            if (!(command is System.Data.SqlClient.SqlCommand))
            {
                command.CommandText = command.CommandText.Replace('@', ':');
            }
            return command;
        }
        private static string getLoggingInfo(System.Data.IDbCommand command)
        {
            return string.Format("{0} called with arguments: {1}", command.CommandText, Database.getArgumentList(command));
        }
        private static string getArgumentList(System.Data.IDbCommand command)
        {
            System.Text.StringBuilder argumentList = new System.Text.StringBuilder();
            foreach (System.Data.Common.DbParameter parameter in command.Parameters)
            {
                argumentList.Append(string.Format("{0}={1};", parameter.ParameterName, parameter.Value));
            }
            if (argumentList.Length == 0)
            {
                argumentList.Append("None");
            }
            return argumentList.ToString();
        }

       
        private T decoratedDatabaseCall<T>(Func<DbCommand, T> delegateMethod, DbCommand command)
        {
            T result = default(T);
            
            try
            {
                result = delegateMethod(command);
            }
            catch (System.Exception ex)
            {
                string message = string.Format("An error - {0} - occured when executing {1}", ex.Message, command.GetLogMessage());
                throw new System.Exception(message, ex);
            }
            return result;
        }
        
        private void addInParameterFrom<T>(DbCommand command, T input, string parameter, System.Reflection.PropertyInfo property)
        {
            object value = DatabaseUtilities.GetDbValue(property.GetValue(input, null), command);
            this.AddInParameter(command, parameter, typeof(System.DateTime?).IsAssignableFrom(property.PropertyType) ? System.Data.DbType.DateTime : System.Data.DbType.AnsiString, value);
        }
       
    }
}
