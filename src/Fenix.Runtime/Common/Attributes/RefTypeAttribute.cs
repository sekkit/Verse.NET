using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{

    public class RefTypeAttribute : Attribute
    {
        public Type Type { get; set; }

        public RefTypeAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
