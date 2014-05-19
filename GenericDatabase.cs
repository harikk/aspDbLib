using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VICommons.DbLib
{
    public abstract class GenericDatabase
    {
        public static string ConnectionString;

        public abstract DbConnection GetOpenedConnection();

        public abstract DbParameter GetDBCellAsDBParameter(DbCommand cmd, DBCell cell);

        public abstract object GetNewValueForType(DbDataType type);

        public abstract SqlDbType GetTypeAsDBType(DbDataType type);

        public abstract string GetAsParameterName(string str);
    }
}
