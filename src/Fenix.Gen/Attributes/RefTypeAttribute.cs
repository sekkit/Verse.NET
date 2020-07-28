using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{

    public class RefTypeAttribute : Attribute
    {
        public string TypeName { get; set; }

        public RefTypeAttribute(string typeName)
        {
            this.TypeName = typeName;
        }
    }
}
