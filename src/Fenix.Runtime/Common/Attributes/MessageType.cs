using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    public class MessageTypeAttribute : Attribute
    {
        public int ProtoCode;

        public MessageTypeAttribute(int protoCode)
        {
            this.ProtoCode = protoCode;
        } 
    }
}
