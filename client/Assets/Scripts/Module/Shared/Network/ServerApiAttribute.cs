using System;

namespace Module.Shared
{
    public class ServerApiAttribute : Attribute
    {
        public ProtoCode Code;
        public ServerApiAttribute(ProtoCode code)
        {
            Code = code;
        }
    }
}