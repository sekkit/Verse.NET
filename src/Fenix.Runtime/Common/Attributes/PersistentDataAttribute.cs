using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PersistentDataAttribute : Attribute
    {
        public Type dataType;
        public string dbName;
        public PersistentDataAttribute(Type type, string dbName)
        {
            this.dataType = type;
            this.dbName = dbName;
        } 
    }
}
