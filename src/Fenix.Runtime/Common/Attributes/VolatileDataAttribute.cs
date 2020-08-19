using System;
namespace Fenix.Common.Attributes
{
    
    public class VolatileDataAttribute : Attribute
    {
        public Type DataType;

        public VolatileDataAttribute(Type type)
        {
            this.DataType = type;
        }
    }

}
