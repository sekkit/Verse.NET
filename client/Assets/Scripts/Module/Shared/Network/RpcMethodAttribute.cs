using System;

namespace Module.Shared
{
    public class RpcMethodAttribute : Attribute
    {
        public ProtoCode Code;
        public RpcMethodAttribute(ProtoCode code)
        {
            Code = code;
        }
    }
}