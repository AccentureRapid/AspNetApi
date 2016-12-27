using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common.Data
{
    public interface IDatabase
    {
        string ConnectionString { get; set; }
        void AddInParameter(DbCommand command, string name, DbType dbType, object value);
        void AddInParameter(DbCommand command, string name, DbType dbType, string value, bool convertDataType);
        void AddOutParameter(DbCommand command, string name, DbType dbType, int size);
        void AddParametersFrom<T>(DbCommand command, T input);
        void AddParametersFrom<T>(DbCommand command, T input, params string[] expectedMatches);
        DbCommandBuilder CreateCommandBuilder();
        int Delete<T>(int id);
        List<T> ExecuteAndReturn<T>(DbCommand command);
        List<T> ExecuteAndReturn<T>(DbCommand command, params ColumnNameMapping[] customNameMappings) where T : new();
        List<T> ExecuteAndReturn<T>(DbCommand command, params string[] autoMapColumns) where T : new();
        List<T> ExecuteAndReturn<T>(DbCommand command, Action<T, IDataRecord> customMapper, params string[] autoMapColumns) where T : new();
        DataSet ExecuteDataSet(DbCommand command);
        int ExecuteNonQueryCommand(DbCommand command);
        IDataReader ExecuteReader(DbCommand command);
        object ExecuteScalarCommand(DbCommand command);
        T Get<T>(int id);
        List<T> GetAll<T>();
        DbDataAdapter GetDataAdapter();
        DbCommand GetSqlStringCommand(string sqlStatement);
        DbCommand GetStoredProcCommand(string storedProcedureName);
        int Insert(object obj);
        int Update(object obj);
        int UpdateWithConcurrencyCheck(DbCommand updateCommand, string timestampColumn, DateTime? originalTimestamp);
    }
}
