using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    public class MessageTypeAttribute : Attribute
    {
        public uint ProtoCode;

        public MessageTypeAttribute(uint protoCode)
        {
            this.ProtoCode = protoCode;
        }
    }
}
