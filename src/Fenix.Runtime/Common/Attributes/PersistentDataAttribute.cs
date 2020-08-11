using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PersistentDataAttribute : Attribute
    {
        public Type dataType;
        public PersistentDataAttribute(Type type)
        {
            dataType = type;
        } 
    }
}
