using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.KCP;
using DotNetty.KCP.Base;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Fenix.Common;
using Fenix.Common.Utils;
using Fenix.Config;

namespace Fenix
{
    public class NetManager
    { 
        protected static ConcurrentDictionary<uint, NetPeer> tcpPeers = new ConcurrentDictionary<uint, NetPeer>();

        protected static ConcurrentDictionary<uint, NetPeer> kcpPeers = new ConcurrentDictionary<uint, NetPeer>();

        protected static ConcurrentDictionary<uint, NetPeer> channelPeers = new ConcurrentDictionary<uint, NetPeer>(); 

        public static NetManager Instance = new NetManager();

        public event Action<NetPeer> OnConnect;
        public event Action<NetPeer, IByteBuffer> OnReceive;
        public event Action<NetPeer> OnClose;
        public event Action<NetPeer, Exception> OnException;
        public event Action<NetPeer, IByteBuffer> OnSend;

        public void RegisterChannel(IChannel channel)
        { 
            var cid = channel.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            var peer = NetPeer.Create(id, channel);
            channelPeers[id] = peer; 
        }

        public void DeregisterChannel(IChannel ch)
        { 
            var peer = GetPeer(ch);   
            this.Deregister(peer); 
        }

        public void ChangePeerId(uint oldHostId, uint newHostId, string hostName, string address)
        {
            Log.Info(string.Format("ChangePeer: {0}=>{1} {2} {3}", oldHostId, newHostId, hostName, address));
            if (tcpPeers.ContainsKey(oldHostId))
            {
                var peer = tcpPeers[oldHostId];
                peer.ConnId = newHostId;
                tcpPeers.TryRemove(oldHostId, out var _);
                tcpPeers[newHostId] = peer;
                var hostInfo = Global.IdManager.GetHostInfo(oldHostId);
                Global.IdManager.RemoveHostId(oldHostId);
                Global.IdManager.RegisterHost(newHostId, hostName, address);
                hostInfo.HostId = newHostId;
                hostInfo.HostName = hostName;
                hostInfo.HostAddr = address;
                Global.IdManager.RegisterHostInfo(hostInfo); 
            }
            if (kcpPeers.ContainsKey(oldHostId))
            {
                var peer = kcpPeers[oldHostId];
                peer.ConnId = newHostId;
                kcpPeers.TryRemove(oldHostId, out var _);
                kcpPeers[newHostId] = peer;
                var hostInfo = Global.IdManager.GetHostInfo(oldHostId);
                Global.IdManager.RemoveHostId(oldHostId);
                Global.IdManager.RegisterHost(newHostId, hostName, address);
                hostInfo.HostId = newHostId;
                hostInfo.HostName = hostName;
                hostInfo.HostAddr = address;
                Global.IdManager.RegisterHostInfo(hostInfo); 
            } 
            if (channelPeers.ContainsKey(oldHostId))
            {
                var peer = channelPeers[oldHostId];
                peer.ConnId = newHostId; 
                if (peer.networkType == NetworkType.KCP)
                    kcpPeers[newHostId] = peer;
                else
                    tcpPeers[newHostId] = peer;
                 
                Global.IdManager.RegisterHost(newHostId, hostName, address); 
            }
        } 

        //kcp目前不支持epoll/kqueue/IOCP，所以只在客户端上用用
        public void RegisterKcp(Ukcp ukcp)
        { 
            var cid = ukcp.user().Channel.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            var peer = NetPeer.Create(id, ukcp);
            channelPeers[peer.ConnId] = peer;
            Log.Info(string.Format("Incoming KCP id: {0}", id));
        }  

        public void DeregisterKcp(Ukcp ukcp)
        { 
            var peer = GetPeer(ukcp);
            this.Deregister(peer);
        }

        public void Deregister(NetPeer peer)
        {
            if (peer == null)
                return; 

            peer.OnClose -= this.OnClose;
            peer.OnReceive -= OnReceive;
            peer.OnException -= OnException;
            
            peer.Stop();

            if (peer.networkType == NetworkType.KCP) 
            { 
                kcpPeers.TryRemove(peer.ConnId, out var _); 
            }

            if (peer.networkType == NetworkType.TCP)
            { 
                tcpPeers.TryRemove(peer.ConnId, out var _);
            }

            if (channelPeers.ContainsKey(peer.ConnId))
                channelPeers.TryRemove(peer.ConnId, out var _);

            Console.WriteLine(string.Format("DeregisterPeer: {0} {1} {2}", peer.ConnId, peer.RemoteAddress, peer.networkType));
        }

#if !CLIENT
        public void RegisterClient(uint clientId, string uniqueName, NetPeer peer)
        {
            var addr = peer.RemoteAddress.ToString();
            if (peer.networkType == NetworkType.KCP)
                kcpPeers[clientId] = peer;
            else if (peer.networkType == NetworkType.TCP)
                tcpPeers[clientId] = peer; 
            Global.IdManager.RegisterClientHost(clientId, uniqueName, peer.RemoteAddress.ToString()); 
        }
#endif

        public void RemovePeerId(uint connId)
        {
            tcpPeers.TryRemove(connId, out var _);
            kcpPeers.TryRemove(connId, out var _);
            Global.IdManager.RemoveHostId(connId);
        }

        public NetPeer GetPeer(IChannel ch)
        {
            var cid = ch.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            return channelPeers[id];
        }

        public NetPeer GetPeer(Ukcp ukcp)
        {  
            var cid = ukcp.user().Channel.Id.AsLongText();
            
            var id = Basic.GenID32FromName(cid);
            return channelPeers[id];
        }

        public NetPeer GetPeerById(uint peerId, NetworkType netType)
        {
            NetPeer peer;
            if (netType == NetworkType.TCP)
            {
                if (tcpPeers.TryGetValue(peerId, out peer))
                    return peer;
            }
            else
            {
                if (kcpPeers.TryGetValue(peerId, out peer))
                    return peer;
            }

            if (channelPeers.ContainsKey(peerId))
                channelPeers.TryGetValue(peerId, out peer);

            return peer;
        }

        public NetPeer CreatePeer(uint remoteHostId, IPEndPoint addr, NetworkType netType)
        { 
            var peer = GetPeerById(remoteHostId, netType);
            if (peer != null)
                return peer;
            peer = NetPeer.Create(remoteHostId, addr, netType);
            if (peer == null)
                return null;
            peer.OnClose += this.OnClose;
            peer.OnReceive += OnReceive;
            peer.OnException += OnException;
            if (netType == NetworkType.TCP)
                tcpPeers[peer.ConnId] = peer;
            else
                kcpPeers[peer.ConnId] = peer;
            peer.Register();
            return peer; 
        }

        //peer connects to hosts(interprocesses)
        public NetPeer CreatePeer(string ip, int port, NetworkType netType)
        {
//#if !CLIENT
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            var addr = ep.ToString();

            var hostId = Global.IdManager.GetHostId(addr);
            if (hostId != 0)
                return NetManager.Instance.GetPeerById(hostId, netType);
//#endif

            var peer = NetPeer.Create(ep, netType);
            if (peer == null)
                return null;

            peer.OnClose += this.OnClose;
            peer.OnReceive += OnReceive;
            peer.OnException += OnException; 

            if (netType == NetworkType.TCP)
                tcpPeers[peer.ConnId] = peer;
            else
                kcpPeers[peer.ConnId] = peer;
            peer.Register();
            return peer;
        }

        //服务端都是tcp，所以不需要心跳？暂时都是发ping/pong包，5s一次，5次没收到就断开 
        public void Ping()
        {
            foreach (var p in tcpPeers.Values)
                p?.Ping();

            foreach (var p in kcpPeers.Values)
                p?.Ping();
        }

        public void OnPong(NetPeer peer)
        {
            peer.lastTickTime = TimeUtil.GetTimeStampMS();
            Console.WriteLine(string.Format("PONG({0}) {1} from {2}", peer.networkType, peer.ConnId, peer.RemoteAddress.ToString()));
        }

        public void Update()
        {
            CheckPeers(tcpPeers.Values);
            CheckPeers(kcpPeers.Values); 
        }

        void CheckPeers(ICollection<NetPeer> peers)
        {
            var curTS = TimeUtil.GetTimeStampMS(); 
            foreach (var p in peers)
            {
                if (p.IsActive == false)
                {
                    Console.WriteLine(string.Format("Remove: {0} {1} {2}", p.ConnId, p.RemoteAddress, p.networkType));
                    this.Deregister(p);
                    continue;
                }

                if (curTS - p.lastTickTime >= RuntimeConfig.HeartbeatIntervalMS * 3)
                {
                    this.PrintPeerInfo("SEKKIT");
                    Console.WriteLine(string.Format("Timeout: {0} {1} {2}", p.ConnId, p.RemoteAddress, p.networkType));
                    this.Deregister(p);
                }
            }
        }

        public void PrintPeerInfo(string header="")
        {
            if (header != "")
                Console.WriteLine(header);

            foreach (var p in tcpPeers.Values)
            {
                Console.WriteLine(string.Format("========Peer({0}): {1} {2} {3} active:{4}", p.networkType, p.ConnId, p.RemoteAddress, p.LocalAddress, p.IsActive)); 
            }

            foreach (var p in kcpPeers.Values)
            {
                Console.WriteLine(string.Format("========Peer({0}): {1} {2} {3} active:{4}", p.networkType, p.ConnId, p.RemoteAddress, p.LocalAddress, p.IsActive));
            }
        }
    }
}
