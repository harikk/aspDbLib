using System;

namespace VICommons.DbLib
{
    public class DBCell
    {
        private string _name;
        private DbDataType _type;
        private object _value;
        private bool _updatable;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool IsUpdatable
        {
            get
            {
                return _updatable;
            }
            set
            {
                _updatable = value;
            }
        }

        public DbDataType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public DBCell(string name, DbDataType type, object value, bool isUpdatable = true)
        {
            Name = name;
            Type = type;
            Value = value;
            IsUpdatable = isUpdatable;
        }
    }

    public enum DbDataType
    {
        Unknown = 0,
        Int = 1,
        VarChar = 2,
        DateTime = 3,
        Date = 4
    }
}
