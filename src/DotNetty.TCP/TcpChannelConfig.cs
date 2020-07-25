using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetty.TCP
{ 
    public class TcpChannelConfig
    {
#if !UNITY_5_3_OR_NEWER
        public bool UseLibuv = false;
#endif

        public bool UseSSL = false;

        public string Address = "127.0.0.1";

        public int Port = 0;
    }
}
