using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VICommons.DbLib.Drivers;

namespace VICommons.DbLib
{
    public abstract class GenericTable<T> : GenericDbModel
    {
        object db = (T)Activator.CreateInstance(typeof(T), null);

        public int Insert()
        {
            return base.Insert((GenericDatabase)db);
        }

        public int Update(String conditions)
        {
            return base.Update((GenericDatabase)db, conditions);
        }

        public int Delete(String conditions)
        {
            return base.Delete((GenericDatabase)db, conditions);
        }

        public bool Load(string conditions)
        {
            return base.Load((GenericDatabase)db, conditions);
        }

        public int Save()
        {
            return base.Save((GenericDatabase)db);
        }

        public DataTable GetFields(string columns, string condetions = null)
        {
            return base.GetFields((GenericDatabase)db, columns, condetions);
        }

        public bool LoadWithPrimaryKey(object primaryKeyValue)
        {
            return base.LoadWithPrimaryKey((GenericDatabase)db, primaryKeyValue);
        }
    }
}
