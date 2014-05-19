using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace VICommons.DbLib.Drivers
{
    public class SqlServerDatabase : GenericDatabase
    {
        public override DbConnection GetOpenedConnection()
        {
            DbConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public override DbParameter GetDBCellAsDBParameter(DbCommand cmd, DBCell cell)
        {
            SqlParameter param = (SqlParameter)cmd.CreateParameter();
            param.ParameterName = cell.Name;
            if (cell.Type == DbDataType.Unknown)
            {
                param.SqlDbType = GetTypeAsDBType(cell.Type);
            }
            else
            {
                param.SqlDbType = GetTypeAsDBType(cell.Type);
            }
            if (cell.Value == null)
            {
                param.Value = GetNewValueForType(cell.Type);
            }
            else
            {
                param.Value = cell.Value;
            }
            return param;
        }

        public override object GetNewValueForType(DbDataType type)
        {
            object newValue;
            switch (type)
            {
                case DbDataType.Int:
                    {
                        newValue = -1;
                        break;
                    }
                case DbDataType.VarChar:
                    {
                        newValue = "";
                        break;
                    }
                case DbDataType.DateTime:
                    {
                        newValue = new DateTime(1753, 1, 1);
                        break;
                    }
                case DbDataType.Date:
                    {
                        newValue = new DateTime(1753, 1, 1);
                        break;
                    }
                default:
                    {
                        throw new TypeAccessException();
                    }
            }
            return newValue;
        }

        public override SqlDbType GetTypeAsDBType(DbDataType type)
        {
            SqlDbType dbType = SqlDbType.Variant;
            switch (type)
            {
                case DbDataType.Int:
                    {
                        dbType = SqlDbType.Int;
                        break;
                    }
                case DbDataType.VarChar:
                    {
                        dbType = SqlDbType.VarChar;
                        break;
                    }
                case DbDataType.DateTime:
                    {
                        dbType = SqlDbType.DateTime;
                        break;
                    }
                case DbDataType.Date:
                    {
                        dbType = SqlDbType.Date;
                        break;
                    }
                default:
                    {
                        throw new TypeAccessException();
                    }
            }
            return dbType;
        }

        public override string GetAsParameterName(string str)
        {
            return "@" + str;
        }
    }
}