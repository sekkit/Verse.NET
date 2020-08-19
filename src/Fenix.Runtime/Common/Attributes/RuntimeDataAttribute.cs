using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RuntimeDataAttribute : Attribute
    {
        public Type DataType;
        public RuntimeDataAttribute(Type type)
        {
            DataType = type;
        }
    }
}
