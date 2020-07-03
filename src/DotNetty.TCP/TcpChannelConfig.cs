using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetty.TCP
{ 
    public class TcpChannelConfig
    {
        public bool UseLibuv = false;

        public bool UseSSL = false;

        public string Address = "127.0.0.1";

        public int Port = 0;
    }
}
