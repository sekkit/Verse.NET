using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using DotNetty.Transport.Channels.Sockets;

namespace DotNetty.KCP
{
    public class ClientEndPointChannelManager : IChannelManager
    {
        private readonly ConcurrentDictionary<EndPoint, Ukcp> _ukcps = new System.Collections.Concurrent.ConcurrentDictionary<EndPoint, Ukcp>();

        public Ukcp get(DatagramPacket msg)
        {
            _ukcps.TryGetValue(msg.Recipient, out var ukcp);
            return ukcp;
        }

        public void New(EndPoint endPoint, Ukcp ukcp, DatagramPacket msg)
        {
            _ukcps[endPoint] = ukcp;
        }

        public void del(Ukcp ukcp)
        {
            if(!_ukcps.TryRemove(ukcp.user().LocalAddress, out var temp))
            {
#if !UNITY_5_3_OR_NEWER
                Console.WriteLine("ukcp session is not exist RemoteAddress: " + ukcp.user().RemoteAddress);
#else
                UnityEngine.Debug.Log("ukcp session is not exist RemoteAddress: " + ukcp.user().RemoteAddress);
#endif
                
            }
        }

        public ICollection<Ukcp> getAll()
        {
            return _ukcps.Values;
        }
    }
}