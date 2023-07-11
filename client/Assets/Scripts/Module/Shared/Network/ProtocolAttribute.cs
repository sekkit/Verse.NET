using System;

namespace Module.Shared
{
    public class ProtocolAttribute : Attribute
    {
        public ProtoCode Code { get; set; }

        public ProtocolAttribute(ProtoCode code)
        {
            Code = code;
        }
    }
}