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



namespace Fenix
{
    public class NetManager
    { 
        protected static ConcurrentDictionary<uint, NetPeer> tcpPeers = new ConcurrentDictionary<uint, NetPeer>();

        protected static ConcurrentDictionary<uint, NetPeer> kcpPeers = new ConcurrentDictionary<uint, NetPeer>();

        protected static ConcurrentDictionary<uint, NetPeer> clientPeers = new ConcurrentDictionary<uint, NetPeer>();

        public static NetManager Instance = new NetManager();

        public event Action<NetPeer> OnConnect;
        public event Action<NetPeer, IByteBuffer> OnReceive;
        public event Action<NetPeer> OnClose;
        public event Action<NetPeer, Exception> OnException;
        public event Action<NetPeer, IByteBuffer> OnSend;

        public void RegisterChannel(IChannel channel)
        { 
            var addr = channel.RemoteAddress.ToString();

            var id = Global.IdManager.GetHostId(addr);
            if (id == 0) 
                id = Basic.GenID32FromName(addr);

            //Global.IdManager.RegisterAddress(id, addr);

            var peer = NetPeer.Create(id, channel);
            //peer.OnClose += this.OnClose;
            //peer.OnReceive += OnReceive;
            //peer.OnException += OnException;
            tcpPeers[peer.ConnId] = peer;
        }

        public void DeregisterChannel(IChannel ch)
        {
            var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString()); 
            tcpPeers.TryRemove(id, out NetPeer peer);
            if (id != 0 && peer != null)
            {
                this.Deregister(peer);
                Global.IdManager.RemoveHostId(peer.ConnId);
            }
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
                //Global.IdManager.RegisterAddress(newId, peer.RemoteAddress.ToString());
            }
        }

        //kcp目前不支持epoll/kqueue/IOCP，所以只在客户端上用用
        public void RegisterKcp(Ukcp ukcp)
        { 
            //int conv = ukcp.getConv();
            var ep = ukcp.user().RemoteAddress;
            //var cid = ukcp.user().Channel.Id.AsLongText();

            var addr = ep.ToString();

            var id = Global.IdManager.GetHostId(addr);
            if(id == 0) 
                id = Basic.GenID32FromName(addr); 

            var peer = NetPeer.Create(id, ukcp); 
            kcpPeers[peer.ConnId] = peer;

            Log.Info(string.Format("Incoming KCP id: {0}", id));
        }  

        public void DeregisterKcp(Ukcp ukcp)
        {
            var ep = ukcp.user().RemoteAddress;
            var addr = ep.ToString();

            //this.mAddr2Ukcp.TryRemove(addr, out var _);
            var id = Global.IdManager.GetHostId(addr);
            if(id == 0) 
                id = Basic.GenID32FromName(addr); 
            kcpPeers.TryRemove(id, out var peer);
            this.Deregister(peer);
        }

        public void Deregister(NetPeer peer)
        {
            if (peer == null)
                return;

            var ep = peer.RemoteAddress; 
            var addr = ep.ToString();
             
            var id = Global.IdManager.GetHostId(addr);
            if (id == 0) 
                id = Basic.GenID32FromName(addr);

            peer.OnClose -= this.OnClose;
            peer.OnReceive -= OnReceive;
            peer.OnException -= OnException;

            peer.Stop();

            if (peer.networkType == NetworkType.KCP) 
            {
                kcpPeers.TryRemove(id, out var _);
                kcpPeers.TryRemove(peer.ConnId, out var _);
            }

            if (peer.networkType == NetworkType.TCP)
            {
                tcpPeers.TryRemove(id, out var _);
                tcpPeers.TryRemove(peer.ConnId, out var _);
            }
        }

#if !CLIENT
        public void RegisterClient(uint clientId, string uniqueName, NetPeer peer)
        {
            var addr = peer.RemoteAddress.ToString();
            clientPeers[clientId] = peer;
            clientPeers[Basic.GenID32FromName(addr)] = peer;
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
            var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString());
            if(id == 0) 
                id = Basic.GenID32FromName(ch.RemoteAddress.ToString()); 
            return tcpPeers[id];
        }

        public NetPeer GetPeer(Ukcp ukcp)
        {
            var ep = ukcp.user().RemoteAddress; 

            var addr = ep.ToString();

            var id = Global.IdManager.GetHostId(addr);
            if (id == 0) 
                id = Basic.GenID32FromName(addr); 

            return kcpPeers[id];
        }

        public NetPeer GetPeerById(uint peerId, NetworkType netType)
        {
            if (netType == NetworkType.TCP)
            {
                if (tcpPeers.TryGetValue(peerId, out var peer))
                    return peer;
                return null;
            }
            else 
            {
                if (kcpPeers.TryGetValue(peerId, out var peer2))
                    return peer2;
                return null;
            }
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

        public void Ping()
        {
            foreach (var p in tcpPeers.Values)
                p?.Ping();

            foreach (var p in kcpPeers.Values)
                p?.Ping();
        }

        public void Update()
        {
            foreach(var p in tcpPeers.Where(m => m.Value.IsActive == false).Select(m => m.Value))
            { 
                Console.WriteLine(string.Format("Remove: {0} {1} {2}", p.ConnId,p.RemoteAddress, p.networkType));
                this.Deregister(p); 
            }

            foreach (var p in kcpPeers.Where(m=>m.Value.IsActive == false).Select(m=>m.Value))
            { 
                Console.WriteLine(string.Format("Remove: {0} {1} {2}", p.ConnId, p.RemoteAddress, p.networkType));
                this.Deregister(p); 
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
