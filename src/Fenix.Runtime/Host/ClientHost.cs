using Fenix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace Fenix
{
    public class ClientHost : Host
    {
        public static ClientHost Create(string ip, int port)
        {
            return new ClientHost(ip, port);
        }

        protected ClientHost(string ip, int port)
            : base(Basic.GenID32FromName(string.Format("{0}:{1}", ip, port)).ToString(), ip, port, true)
        {

        }
    }
}
