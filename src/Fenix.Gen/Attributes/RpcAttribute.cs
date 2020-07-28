using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    //public class RequestAttribute : Attribute
    //{
    //    public UInt32 Code;
    //    public RequestAttribute(UInt32 protoCode)
    //    {
    //        Code = protoCode;
    //    }
    //}

    //public class ResponseAttribute : Attribute
    //{
    //    public UInt32 Code;
    //    public ResponseAttribute(UInt32 protoCode)
    //    {
    //        Code = protoCode;
    //    }
    //}

    public class RpcMethodAttribute : Attribute
    {
        public UInt32 Code;
        public Api Api;

        public RpcMethodAttribute(UInt32 protoCode, Api api)
        {
            this.Code = protoCode;
            this.Api = api;
        } 
    }

    public enum Api : byte
    {
        ServerApi = 1,
        ServerOnly = 2,
        ClientApi = 3,
        NoneApi = 0xff
    }

    public class ServerApiAttribute : Attribute
    {  
        public ServerApiAttribute()
        { 
        } 
    }

    public class ClientApiAttribute : Attribute
    {
        public ClientApiAttribute()
        {
        }
    }

    public class ServerOnlyAttribute : Attribute
    {
        public ServerOnlyAttribute()
        {
        }
    }

    public class RpcAttribute : Attribute
    {
        public Api RpcType { get; set; }
        public RpcAttribute(Api api)
        {
            RpcType = api;
        }
    }
}
