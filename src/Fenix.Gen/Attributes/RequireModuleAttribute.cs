using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireModuleAttribute : Attribute
    {
        public Type ModuleType;

        public RequireModuleAttribute(Type type)
        {
            this.ModuleType = type;
        }
    }
}
