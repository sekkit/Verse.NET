using Fenix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ServerHost : Host
    {
        public static ServerHost Create(string name, string ip, int port)
        {
            return new ServerHost(name, ip, port);
        }

        public ServerHost(string name, string ip, int port)
            : base(name, ip, port, false)
        {
        }
    }
}
