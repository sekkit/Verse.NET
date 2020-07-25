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

        //protected static ConcurrentDictionary<uint, NetPeer> clientPeers = new ConcurrentDictionary<uint, NetPeer>();

        public static NetManager Instance = new NetManager();

        public event Action<NetPeer> OnConnect;
        public event Action<NetPeer, IByteBuffer> OnReceive;
        public event Action<NetPeer> OnClose;
        public event Action<NetPeer, Exception> OnException;
        public event Action<NetPeer, IByteBuffer> OnSend;

        public void RegisterChannel(IChannel channel)
        {
            //var addr = channel.RemoteAddress.ToString();
            
            var cid = channel.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            var peer = NetPeer.Create(id, channel);
            channelPeers[id] = peer;

            //var id = Global.IdManager.GetHostId(addr);
            //if (id == 0) 
            //    id = Basic.GenID32FromName(addr);

            ////Global.IdManager.RegisterAddress(id, addr);

            //var peer = NetPeer.Create(id, channel);
            ////peer.OnClose += this.OnClose;
            ////peer.OnReceive += OnReceive;
            ////peer.OnException += OnException;
            //tcpPeers[peer.ConnId] = peer;
        }

        public void DeregisterChannel(IChannel ch)
        {
            //var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString());
            var peer = GetPeer(ch);  
            //tcpPeers.TryRemove(peer.ConnId, out NetPeer _);
            //if (id != 0 && peer != null)
            //{
            this.Deregister(peer);
            //Global.IdManager.RemoveHostId(peer.ConnId);
            //}
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
                //Global.IdManager.RegisterAddress(newId, peer.RemoteAddress.ToString());
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
                //channelPeers.TryRemove(oldHostId, out var _);
                if (peer.networkType == NetworkType.KCP)
                    kcpPeers[newHostId] = peer;
                else
                    tcpPeers[newHostId] = peer;

                //var hostInfo = Global.IdManager.GetHostInfo(newHostId); 
                //Global.IdManager.RemoveHostId(oldHostId);
                Global.IdManager.RegisterHost(newHostId, hostName, address);

                //hostInfo.HostId = newHostId;
                //hostInfo.HostName = hostName;
                //hostInfo.HostAddr = address;
                //Global.IdManager.RegisterHostInfo(hostInfo); 
            }
        }

        //public void ChangePeerId(string channelId, uint newHostId, string hostName, string address)
        //{
        //    Log.Info(string.Format("ChangePeer: {0}=>{1} {2} {3}", channelId, newHostId, hostName, address));
        //    if (channelPeers.ContainsKey(channelId))
        //    {
        //        var peer = channelPeers[channelId];
        //        peer.ConnId = newHostId;
        //        channelPeers.TryRemove(channelId, out var _);
        //        if (peer.networkType == NetworkType.KCP)
        //            kcpPeers[newHostId] = peer;
        //        else
        //            tcpPeers[newHostId] = peer;

        //        //var hostInfo = Global.IdManager.GetHostInfo(newHostId); 
        //        //Global.IdManager.RemoveHostId(oldHostId);
        //        Global.IdManager.RegisterHost(newHostId, hostName, address);

        //        //hostInfo.HostId = newHostId;
        //        //hostInfo.HostName = hostName;
        //        //hostInfo.HostAddr = address;
        //        //Global.IdManager.RegisterHostInfo(hostInfo); 
        //    } 
        //}

        //kcp目前不支持epoll/kqueue/IOCP，所以只在客户端上用用
        public void RegisterKcp(Ukcp ukcp)
        {
            ////int conv = ukcp.getConv();
            //var ep = ukcp.user().RemoteAddress;
            ////var cid = ukcp.user().Channel.Id.AsLongText();

            //var addr = ep.ToString();

            //var id = Global.IdManager.GetHostId(addr);
            //if(id == 0) 
            //    id = Basic.GenID32FromName(addr); 

            //var peer = NetPeer.Create(id, ukcp); 
            //kcpPeers[peer.ConnId] = peer;

            var cid = ukcp.user().Channel.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            var peer = NetPeer.Create(id, ukcp);
            channelPeers[peer.ConnId] = peer;
            Log.Info(string.Format("Incoming KCP id: {0}", id));
        }  

        public void DeregisterKcp(Ukcp ukcp)
        {
            //var ep = ukcp.user().RemoteAddress;
            //var addr = ep.ToString();

            ////this.mAddr2Ukcp.TryRemove(addr, out var _);
            //var id = Global.IdManager.GetHostId(addr);
            //if(id == 0) 
            //    id = Basic.GenID32FromName(addr); 
            //kcpPeers.TryRemove(id, out var peer);
            ////clientPeers.TryRemove(id, out var _); 
            //var cid = ukcp.user().Channel.Id.AsLongText();
            //var id = Basic.GenID32FromName(cid);
            var peer = GetPeer(ukcp);
            this.Deregister(peer);
        }

        public void Deregister(NetPeer peer)
        {
            if (peer == null)
                return;

            //var ep = peer.RemoteAddress; 
            //var addr = ep.ToString();
             
            //var id = Global.IdManager.GetHostId(addr);
            //if (id == 0) 
            //    id = Basic.GenID32FromName(addr);

            peer.OnClose -= this.OnClose;
            peer.OnReceive -= OnReceive;
            peer.OnException -= OnException;

            peer.Stop();

            if (peer.networkType == NetworkType.KCP) 
            {
                //kcpPeers.TryRemove(id, out var _);
                kcpPeers.TryRemove(peer.ConnId, out var _);
                //clientPeers.TryRemove(id, out var _);
                //clientPeers.TryRemove(peer.ConnId, out var _);
            }

            if (peer.networkType == NetworkType.TCP)
            {
                //tcpPeers.TryRemove(id, out var _);
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
            //clientPeers[clientId] = peer;
            //clientPeers[Basic.GenID32FromName(addr)] = peer;
            Global.IdManager.RegisterClientHost(clientId, uniqueName, peer.RemoteAddress.ToString()); 
        }
#endif

        public void RemovePeerId(uint connId)
        {
            tcpPeers.TryRemove(connId, out var _);
            kcpPeers.TryRemove(connId, out var _);
            //clientPeers.TryRemove(connId, out var _);
            Global.IdManager.RemoveHostId(connId);
        }

        public NetPeer GetPeer(IChannel ch)
        {
            var cid = ch.Id.AsLongText();
            var id = Basic.GenID32FromName(cid);
            return channelPeers[id];

            //var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString());
            //if(id == 0) 
            //    id = Basic.GenID32FromName(ch.RemoteAddress.ToString()); 
            //return tcpPeers[id];
        }

        public NetPeer GetPeer(Ukcp ukcp)
        {
            //var ep = ukcp.user().RemoteAddress;  
            //var addr = ep.ToString();

            var cid = ukcp.user().Channel.Id.AsLongText();

            //var id = Global.IdManager.GetHostId(addr);
            //if (id == 0) 
            //    id = Basic.GenID32FromName(addr); 
            //var id = Basic.GenID32FromName(cid); 
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
