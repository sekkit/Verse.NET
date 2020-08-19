using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PersistentDataAttribute : Attribute
    {
        public Type DataType;
        public string dbName;
        public int dbIndex;

#if CLIENT
        public PersistentDataAttribute(Type type)
        {
            this.DataType = type;
            this.dbName = "";
        }
#endif

        public PersistentDataAttribute(Type type, string dbName)
        {
            this.DataType = type;
            this.dbName = dbName;
        }

        public PersistentDataAttribute(Type type, string dbName, int index)
        {
            this.DataType = type;
            this.dbName = dbName;
            this.dbIndex = index;
        }
    }
}
