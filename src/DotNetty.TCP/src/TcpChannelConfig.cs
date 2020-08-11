using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotNetty.TCP
{ 
    public class TcpChannelConfig
    {
#if !CLIENT
        public bool UseLibuv = false;
#endif

        public bool UseSSL = false;

        //public string Address = "127.0.0.1";

        //public int Port = 0;

        public IPEndPoint Address;
    }
}
