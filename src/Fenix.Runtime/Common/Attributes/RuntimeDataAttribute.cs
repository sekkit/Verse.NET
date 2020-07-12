using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{ 
    public class RuntimeDataAttribute : Attribute
    {
        public Type dataType;
        public RuntimeDataAttribute(Type type)
        {
            dataType = type;
        }
    }
}
