using System;
using System.Data;
using System.Data.Common;

namespace VICommons.DbLib
{
    protected abstract class GenericDbModel
    {
        #region Properties
        public abstract string TableName
        {
            get;
        }

        public abstract DBCell PrimaryKey
        {
            get;
        }

        public abstract DBCell[] Values
        {
            get;
        }
        #endregion
        #region Indexers
        public DBCell this[string name]
        {
            get
            {
                int i = GetColumnIndex(name);
                if (i == -1)
                {
                    throw new IndexOutOfRangeException();
                }
                return Values[i];
            }
            set
            {
                int i = GetColumnIndex(name);
                if (i == -1)
                {
                    throw new IndexOutOfRangeException();
                }
                Values[i] = value;
            }
        }
        #endregion

        public int Insert(GenericDatabase db)
        {
            DbCommand dbCommand = GetInsertCommand(db, db.GetOpenedConnection(), TableName, Values);
            return dbCommand.ExecuteNonQuery();
        }

        public int Update(GenericDatabase db, String conditions)
        {
            DbCommand dbCommand = GetUpdateCommand(db, db.GetOpenedConnection(), TableName, Values);
            if (conditions != null)
            {
                dbCommand.CommandText += " WHERE " + conditions;
            }
            return dbCommand.ExecuteNonQuery();
        }

        public int Delete(GenericDatabase db, String conditions)
        {
            string sql = "DELETE FROM " + TableName + " WHERE " + conditions;
            DbCommand dbCommand = db.GetOpenedConnection().CreateCommand();
            dbCommand.CommandText = sql;
            return dbCommand.ExecuteNonQuery();
        }

        public bool Load(GenericDatabase db, string conditions)
        {
            DbCommand dbCommand = db.GetOpenedConnection().CreateCommand();
            dbCommand.CommandText = "SELECT * FROM " + TableName + " WHERE " + conditions;
            return FillObjectWithDataReader(dbCommand);
        }

        public int Save(GenericDatabase db)
        {
            if (isValueExistsWithColumns(db, new string[] { PrimaryKey.Name }))
            {
                return Update(db, new DBCell[] { PrimaryKey });
            }
            else
            {
                return Insert(db);
            }
        }
        
        public DataTable GetFields(GenericDatabase db, string columns, string condetions = null)
        {
            DbCommand dbCommand = db.GetOpenedConnection().CreateCommand();
            dbCommand.CommandText = "SELECT " + columns + " FROM " + TableName;
            if (condetions != null)
            {
                dbCommand.CommandText += " WHERE " + condetions;
            }
            return GetDataTableFromCommand(dbCommand);
        }

        public bool LoadWithPrimaryKey(GenericDatabase db, object primaryKeyValue)
        {
            DbCommand dbCommand = db.GetOpenedConnection().CreateCommand();
            dbCommand.CommandText = "SELECT * FROM " + TableName + " WHERE " + PrimaryKey.Name + " = " + db.GetAsParameterName(PrimaryKey.Name);
            dbCommand.Parameters.Add(db.GetDBCellAsDBParameter(dbCommand, new DBCell(PrimaryKey.Name, PrimaryKey.Type, primaryKeyValue)));
            return FillObjectWithDataReader(dbCommand);
        }
        
        public bool Validate()
        {
            bool validationSuccess = true;
            foreach(DBCell cell in Values)
            {
                validationSuccess = validationSuccess && Validate(cell);
                if (validationSuccess == false)
                {
                    throw new Exception("Value of " + cell.Name + " is not " + cell.Type.ToString());
                }
            }
            return validationSuccess;
        }

        public DataTable GetDataTableFromCommand(DbCommand cmd)
        {
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
        }

        #region Private Memebers
        private int GetColumnIndex(string columnLabel)
        {
            int i = 0;
            for (i = 0; i < Values.Length; i++)
            {
                if (Values[i].Name.Equals(columnLabel, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
            }
            if (i == Values.Length)
            {
                i = -1;
            }
            return i;
        }

        private bool FillObjectWithDataReader(DbCommand reader)
        {
            bool loaded = false;
            DataTable dt = new DataTable();
            dt.Load(reader.ExecuteReader());
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    this[dt.Columns[i].ColumnName].Value = dt.Rows[0][i];
                }
                loaded = true;
            }
            return loaded;
        }

        private bool isValueExistsWithColumns(GenericDatabase db, String[] keyNames)
        {
            bool exists = false;
            if (keyNames == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                if ((keyNames.Length <= 0) || (keyNames.Length > Values.Length))
                {
                    throw new ArgumentException();
                }
                else
                {
                    String sql = "Select TOP 1 * from " + TableName + " where ";
                    DbCommand cmd = db.GetOpenedConnection().CreateCommand();
                    for (int i = 0; i < keyNames.Length; i++)
                    {
                        DBCell cell = this[keyNames[i]];
                        sql += " " + keyNames[i] + " = " + db.GetAsParameterName(keyNames[i]) + " and ";
                        cmd.Parameters.Add(db.GetDBCellAsDBParameter(cmd, cell));
                    }
                    sql += " 1=1";
                    cmd.CommandText = sql;
                    exists = (GetDataTableFromCommand(cmd).Rows.Count > 0);
                }
            }
            return exists;
        }

        private DbCommand GetInsertCommand(GenericDatabase db, DbConnection conn, string tableName, DBCell[] cells)
        {
            if(Validate())
            {
                DbCommand dbCommand = conn.CreateCommand();
                dbCommand.CommandText = "INSERT INTO " + tableName + "(";
                String vals = "";
                for (int i = 0; i < cells.Length; i++)
                {
                    if (cells[i].IsUpdatable)
                    {
                        dbCommand.CommandText += cells[i].Name;
                        dbCommand.CommandText += ", ";
                        vals += db.GetAsParameterName(cells[i].Name);
                        vals += ", ";
                        dbCommand.Parameters.Add(db.GetDBCellAsDBParameter(dbCommand, cells[i]));
                    }
                }
                dbCommand.CommandText = Utils.RemoveTrailingSymbol(dbCommand.CommandText, ",");
                vals = Utils.RemoveTrailingSymbol(vals, ",");
                dbCommand.CommandText += ") VALUES (" + vals + ")";
                return dbCommand;
            }
            return null;
        }

        private DbCommand GetUpdateCommand(GenericDatabase db, DbConnection conn, string tableName, DBCell[] cells)
        {
            if (Validate())
            {
                string sql = "UPDATE " + tableName + " SET ";
                DbCommand dbCommand = conn.CreateCommand();
                for (int i = 0; i < cells.Length; i++)
                {
                    if (cells[i].IsUpdatable)
                    {
                        sql += cells[i].Name + " = " + db.GetAsParameterName(cells[i].Name);
                        sql += ", ";
                        dbCommand.Parameters.Add(db.GetDBCellAsDBParameter(dbCommand, cells[i]));
                    }
                }
                sql = Utils.RemoveTrailingSymbol(sql, ",");
                dbCommand.CommandText = sql;
                return dbCommand;
            }
            return null;
        }

        private DbCommand GetUpdateCommand(GenericDatabase db, DbConnection conn, string tableName, DBCell[] cells, DBCell[] condetions)
        {
            DbCommand cmd = GetUpdateCommand(db, conn, tableName, cells);
            cmd.CommandText += " WHERE ";
            foreach (DBCell condetion in condetions)
            {
                cmd.CommandText += condetion.Name + " = " + db.GetAsParameterName(condetion.Name) + " AND ";
                cmd.Parameters.Add(db.GetDBCellAsDBParameter(cmd, condetion));
            }
            cmd.CommandText += " 1=1";
            return cmd;
        }

        private bool Validate(DBCell cell)
        {
            bool isValid = true;
            try
            {
                switch (cell.Type)
                {
                    case DbDataType.Int:
                        {
                            Convert.ToInt32(cell.Value);
                            break;
                        }
                    case DbDataType.DateTime:
                    case DbDataType.Date:
                        {
                            Convert.ToDateTime(cell.Value);
                            break;
                        }
                    case DbDataType.VarChar:
                        {
                            break;
                        }
                    default:
                        {
                            isValid = false;
                            break;
                        }
                }
            }
            catch
            {
                isValid = false;
            }
            return isValid;
        }
        #endregion
    }
}
