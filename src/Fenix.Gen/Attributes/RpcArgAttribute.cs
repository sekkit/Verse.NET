using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class RpcArgAttribute : Attribute
    {
        public string Name;

        public RpcArgAttribute(string name)
        {
            this.Name = name;
        }
    }
}
