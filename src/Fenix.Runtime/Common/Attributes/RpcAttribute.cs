using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    public class RequestAttribute : Attribute
    {
        public UInt32 Code;
        public RequestAttribute(UInt32 protoCode)
        {
            Code = protoCode;
        }
    }

    public class ResponseAttribute : Attribute
    {
        public UInt32 Code;
        public ResponseAttribute(UInt32 protoCode)
        {
            Code = protoCode;
        }
    }

    public class RpcMethodAttribute : Attribute
    {
        public UInt32 Code;
        public RpcMethodAttribute(UInt32 protoCode)
        {
            Code = protoCode;
        }
    }
}
