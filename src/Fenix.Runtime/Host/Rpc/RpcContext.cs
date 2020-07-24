using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class RpcContext
    {
        public Packet Packet;
        public NetPeer Peer;

        public RpcContext(Packet packet, NetPeer peer)
        {
            Packet = packet;
            Peer = peer;
        }
    }
}
