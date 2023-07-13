using System;

namespace Module.Shared
{
    public class ClientApiAttribute : Attribute
    {
        public ProtoCode Code;
        public ClientApiAttribute(ProtoCode code)
        {
            Code = code;
        }
    }
}