using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common
{
    public class RpcUtil
    {
        public enum Api
        {
            ServerApi=1,
            ServerOnly=2,
            ClientApi=3
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
}
