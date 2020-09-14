using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net; 
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.KCP;
using DotNetty.Transport.Channels;
using Fenix.Common;
using Fenix.Common.Utils;
using Fenix.Config;

namespace Fenix
{
    public class NetManager
    { 
        protected ConcurrentDictionary<ulong, NetPeer> tcpPeers = new ConcurrentDictionary<ulong, NetPeer>();

        protected ConcurrentDictionary<ulong, NetPeer> kcpPeers = new ConcurrentDictionary<ulong, NetPeer>();

        protected ConcurrentDictionary<ulong, NetPeer> channelPeers = new ConcurrentDictionary<ulong, NetPeer>(); 

        public ConcurrentDictionary<ulong, byte[][]> partialRpcDic = new ConcurrentDictionary<ulong, byte[][]>();

        protected ConcurrentDictionary<ulong, long> partialRpcTimeDic = new ConcurrentDictionary<ulong, long>();

        public event Action<NetPeer> OnConnect;
        public event Action<NetPeer, IByteBuffer> OnReceive;
        public event Action<NetPeer> OnClose;
        public event Action<NetPeer, Exception> OnException;
        //public event Action<NetPeer, IByteBuffer> OnSend;
        public event Action OnHeartBeat;
        //public event Action<NetPeer> OnPeerLost;

        protected ConcurrentDictionary<ulong, KcpHostServer> kcpServerDic { get; set; } = new ConcurrentDictionary<ulong, KcpHostServer>();

        protected ConcurrentDictionary<ulong, TcpHostServer> tcpServerDic { get; set; } = new ConcurrentDictionary<ulong, TcpHostServer>();
         
        private Thread heartbeatTh;

        public void RegisterHost(Host host)
        {
            if (!host.IsClientMode)
            {
                var kcpServer = CreateKcpServer(host.ExternalAddress, host.LocalAddress);
                kcpServerDic[host.Id] = kcpServer;

                var tcpServer = this.CreateTcpServer(host.ExternalAddress, host.LocalAddress);
                tcpServerDic[host.Id] = tcpServer;
            }
        }

        public NetManager()
        {
            heartbeatTh = new Thread(new ThreadStart(Heartbeat));
            heartbeatTh.Start();
        }

        protected void Heartbeat()
        {
            while (true)
            {
                Thread.Sleep(5000);
                try
                { 
                    //Log.Info(string.Format("Heartbeat:{0}", IsAlive));
                    PrintPeerInfo();
                    Ping(true);
                    OnHeartBeat?.Invoke();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        #region KCP Server

        protected KcpHostServer CreateKcpServer(IPEndPoint extAddr, IPEndPoint localAddr)
        {
            var kcpServer = KcpHostServer.Create(localAddr);
            kcpServer.OnConnect += KcpServer_OnConnect;
            kcpServer.OnReceive += KcpServer_OnReceive;
            kcpServer.OnClose += KcpServer_OnClose;
            kcpServer.OnException += KcpServer_OnException;

            Log.Info(string.Format("KCP-Server@{0}", localAddr));
            return kcpServer;
        }

        protected void KcpServer_OnConnect(Ukcp ukcp)
        {
            //新连接
            var peer = Global.NetManager.RegisterKcp(ukcp);
            //ulong hostId = Global.IdManager.GetHostId(channel.RemoteAddress.ToIPv4String());
            Log.Info(string.Format("kcp_client_connected {0} {1}",
                ukcp.GetUniqueId(),
                ukcp.user().RemoteAddress.ToIPv4String()));
            OnConnect?.Invoke(peer);
        }

        private void KcpServer_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            var peer = Global.NetManager.GetPeer(ukcp);
            OnReceive?.Invoke(peer, buffer);
        }

        private void KcpServer_OnException(Ukcp ukcp, Exception ex)
        {
            var peer = Global.NetManager.GetPeer(ukcp);
            OnException?.Invoke(peer, ex);
        }

        private void KcpServer_OnClose(Ukcp ukcp)
        {
            var peer = Global.NetManager.GetPeer(ukcp);
            OnClose?.Invoke(peer);
            Deregister(peer); 
        }
        #endregion

        #region TCP Server
        protected TcpHostServer CreateTcpServer(IPEndPoint extAddr, IPEndPoint localAddr)
        {
            var tcpServer = TcpHostServer.Create(localAddr);
            tcpServer.OnConnect += OnTcpConnect;
            tcpServer.OnReceive += OnTcpServerReceive;
            tcpServer.OnClose += OnTcpServerClose;
            tcpServer.OnException += OnTcpServerException;
            Log.Info(string.Format("TCP-Server@{0}", localAddr));
            return tcpServer;
        }

        void OnTcpConnect(IChannel channel)
        {
            //新连接
            var peer = Global.NetManager.RegisterChannel(channel);
            //ulong hostId = Global.IdManager.GetHostId(channel.RemoteAddress.ToIPv4String());
            Log.Info("TcpConnect: " + channel.RemoteAddress.ToIPv4String());

            OnConnect?.Invoke(peer);
        }

        void OnTcpServerReceive(IChannel channel, IByteBuffer buffer)
        {
            var peer = Global.NetManager.GetPeer(channel);
            OnReceive(peer, buffer);
        }

        void OnTcpServerClose(IChannel channel)
        {
            //Global.NetManager.DeregisterChannel(channel);
            var peer = Global.NetManager.GetPeer(channel);
            OnClose?.Invoke(peer);
            Deregister(peer);
        }

        void OnTcpServerException(IChannel channel, Exception ex)
        {
            //Global.NetManager.DeregisterChannel(channel);
            var peer = Global.NetManager.GetPeer(channel);
            OnException?.Invoke(peer, ex);
        }

        #endregion

        public NetPeer RegisterChannel(IChannel channel)
        { 
            var cid = channel.Id.AsLongText();
            var id = Basic.GenID64FromName(cid);
            var peer = NetPeer.Create(id, channel);
            channelPeers[id] = peer;
            //RemotePeers.channelPeers[id] = peer;
            return peer;
        }

        public bool DeregisterChannel(IChannel ch)
        { 
            return this.Deregister(GetPeer(ch)); 
        }

        public void ChangePeerId(ulong oldHostId, ulong newHostId, string hostName, string address)
        {
            Log.Info(string.Format("ChangePeer: {0}=>{1} {2} {3}", oldHostId, newHostId, hostName, address));
            if (tcpPeers.ContainsKey(oldHostId))
            {
                var peer = tcpPeers[oldHostId];
                peer.ConnId = newHostId;
                //tcpPeers.TryRemove(oldHostId, out var _);
                tcpPeers[newHostId] = peer;

                //var hostInfo = Global.IdManager.GetHostInfo(oldHostId);                
                ////Global.IdManager.RemoveHostId(oldHostId);
                ////Global.IdManager.RegisterHost(newHostId, hostName, address);
                //hostInfo.HostId = newHostId;
                //hostInfo.HostName = hostName;
                //hostInfo.HostAddr = address;
                ////Global.IdManager.RegisterHostInfo(hostInfo);
            }

            if (kcpPeers.ContainsKey(oldHostId))
            {
                var peer = kcpPeers[oldHostId];
                peer.ConnId = newHostId;
                //kcpPeers.TryRemove(oldHostId, out var _);
                kcpPeers[newHostId] = peer;

                //var hostInfo = Global.IdManager.GetHostInfo(oldHostId);
                ////Global.IdManager.RemoveHostId(oldHostId);
                ////Global.IdManager.RegisterHost(newHostId, hostName, address);
                //hostInfo.HostId = newHostId;
                //hostInfo.HostName = hostName;
                //hostInfo.HostAddr = address;
                ////Global.IdManager.RegisterHostInfo(hostInfo);
            }

            if (channelPeers.ContainsKey(oldHostId))
            {
                var peer = channelPeers[oldHostId];
                peer.ConnId = newHostId;
                channelPeers[newHostId] = peer;
                //channelPeers.TryRemove(oldHostId, out var _);
#if !CLIENT
                if (Global.Config.DuplexMode)
                {
#endif
                    if (peer.netType == NetworkType.KCP)
                        kcpPeers[newHostId] = peer;
                    else
                        tcpPeers[newHostId] = peer;
#if !CLIENT
                }
#endif

                //Global.IdManager.RegisterHost(newHostId, hostName, address); 
            }
        } 

        //kcp目前不支持epoll/kqueue/IOCP，所以只在客户端上用用
        public NetPeer RegisterKcp(Ukcp ukcp)
        {  
            var id = ukcp.GetUniqueId();
            var peer = NetPeer.Create(id, ukcp);
            channelPeers[peer.ConnId] = peer;
            Log.Info(string.Format("Incoming KCP id: {0}", id));
            return peer;
        }  

        public bool DeregisterKcp(Ukcp ukcp)
        { 
            var peer = GetPeer(ukcp);
            return this.Deregister(peer);
        }

        public bool Deregister(NetPeer peer)
        {
            if (peer == null)
                return false; 
            
            peer.Stop();

            peer.OnClose -= OnClose;
            peer.OnReceive -= OnReceive;
            peer.OnException -= OnException;

            if (peer.netType == NetworkType.KCP)
            {
                kcpPeers.TryRemove(peer.ConnId, out var _);
                foreach(var k in kcpPeers.Where(m => m.Value.ConnId == peer.ConnId).Select(m => m.Key).ToList())
                {
                    kcpPeers.TryRemove(k, out var _);
                }
            }

            if (peer.netType == NetworkType.TCP)
            {
                tcpPeers.TryRemove(peer.ConnId, out var _);
                foreach (var k in tcpPeers.Where(m => m.Value.ConnId == peer.ConnId).Select(m=>m.Key).ToList())
                {
                    tcpPeers.TryRemove(k, out var _);
                }
            }

            if (channelPeers.ContainsKey(peer.ConnId))
            {
                channelPeers.TryRemove(peer.ConnId, out var _);
                foreach (var k in channelPeers.Where(m => m.Value.ConnId == peer.ConnId).Select(m => m.Key).ToList())
                {
                    channelPeers.TryRemove(k, out var _);
                }
            }

            Log.Info(string.Format("DeregisterPeer: {0} {1} {2}", peer.ConnId, peer.RemoteAddress, peer.netType));

            return true;
        }

#if !CLIENT
        public void RegisterClient(ulong clientId, string uniqueName, NetPeer peer)
        {
            var addr = peer.RemoteAddress.ToIPv4String();
            if (peer.netType == NetworkType.KCP)
                kcpPeers[clientId] = peer;
            else if (peer.netType == NetworkType.TCP)
                tcpPeers[clientId] = peer; 
            Global.IdManager.RegisterClientHost(clientId, uniqueName, peer.RemoteAddress.ToIPv4String()); 
        }
#endif

        public void RemovePeerId(ulong connId)
        {
            tcpPeers.TryRemove(connId, out var _);
            kcpPeers.TryRemove(connId, out var _);
            Global.IdManager.RemoveHostId(connId);
        }

        public NetPeer GetPeer(IChannel ch)
        {
            var cid = ch.Id.AsLongText();
            var id = Basic.GenID64FromName(cid);
            return channelPeers[id];
        }

        public NetPeer GetPeer(Ukcp ukcp)
        {
            var id = ukcp.GetUniqueId();
            channelPeers.TryGetValue(id, out var peer);
            return peer;
        }

        public NetPeer GetLocalPeerById(ulong peerId, NetworkType netType)
        {
#if !CLIENT
            if (Global.Config.DuplexMode)
                return GetPeerById(peerId, netType);
#endif

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

            return peer;
        }

        public NetPeer GetRemotePeerById(ulong peerId, NetworkType netType)
        {
#if !CLIENT
            if (Global.Config.DuplexMode)
                return GetPeerById(peerId, netType);
#endif
            NetPeer peer = null;
            channelPeers.TryGetValue(peerId, out peer); 
            return peer;
        }

        public NetPeer GetPeerById(ulong peerId, NetworkType netType)
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
             
            channelPeers.TryGetValue(peerId, out peer);
            return peer;
        }

        public NetPeer CreatePeer(ulong remoteHostId, IPEndPoint addr, NetworkType netType)
        { 
            var peer = GetLocalPeerById(remoteHostId, netType);
            if (peer != null)
                return peer;
            peer = NetPeer.Create(remoteHostId, addr, netType);
            if (peer == null)
                return null;
            peer.OnClose += OnClose;
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
            var ep = new IPEndPoint(IPAddress.Parse(ip), port);
            var addr = ep.ToString();

            var hostId = Global.IdManager.GetHostId(addr);
            if (hostId != 0)
                return Global.NetManager.GetLocalPeerById(hostId, netType);
//#endif

            var peer = NetPeer.Create(ep, netType);
            if (peer == null)
                return null;

            peer.OnClose += OnClose;
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
        public void Ping(bool pingServerOnly)
        {
            foreach (var p in tcpPeers.Values)
                if (pingServerOnly && !p.IsRemoteClient)
                    p?.Ping();
                else if (!pingServerOnly && p.IsRemoteClient)
                    p?.Ping();
            //p?.Ping();

            foreach (var p in kcpPeers.Values)
                if (pingServerOnly && !p.IsRemoteClient)
                    p?.Ping();
                else if (!pingServerOnly && p.IsRemoteClient)
                    p?.Ping();
            //p?.Ping();
        }

        public void OnPong(NetPeer peer)
        {
            peer.lastTickTime = Fenix.Common.Utils.TimeUtil.GetTimeStampMS();
            //Log.Info(string.Format("PONG({0}) {1} from {2}", peer.netType, peer.ConnId, peer.RemoteAddress?.ToString()));
        }

        public void Send(NetPeer peer, Packet packet)
        {
            var bytes = packet.Pack();

            Log.Info("SEND_PROTOCOL", packet.ProtoCode, Global.Host.Id, peer.ConnId, peer.LocalAddress, peer.RemoteAddress, StringUtil.ToHexString(bytes), peer.IsActive, peer.IsAlive);

            if (bytes.Length > Global.Config.MAX_PACKET_SIZE)
            {
                PartialSendAsync(peer, bytes);
                return;
            }

            peer.Send(bytes);
        }

        protected async Task PartialSendAsync(NetPeer peer, byte[] bytes)
        {
            var parts = DataUtil.SplitBytes(bytes, Global.Config.MAX_PACKET_SIZE);
            var partialId = Basic.GenID64();
            var totalPartNum = parts.Count();
            if(totalPartNum > 256)
            {
                Log.Error("send_bytes_too_long", peer.ConnId, totalPartNum);
                return;
            }
            for (short i = 0; i < parts.Count(); ++i)
            {
                var part = parts.ElementAt(i);

                using (var m = new MemoryStream())
                {
                    using (var writer = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Little, m))
                    {
                        writer.Write(OpCode.PARTIAL);
                        writer.Write(partialId);
                        writer.Write(i);
                        writer.Write(parts.Count());
                        writer.Write(part);
                        peer.Send(m.ToArray());
                        await Task.Delay(1);
                        Log.Info("send_part", i, parts.Count(), part.Length);
                    }
                } 

                //var partialBuf = Unpooled.DirectBuffer();
                //partialBuf.WriteIntLE((int)OpCode.PARTIAL);
                //partialBuf.WriteLongLE((long)partialId);
                //partialBuf.WriteByte(i);
                //partialBuf.WriteByte(parts.Count());
                //partialBuf.WriteBytes(part);
                //peer.Send(partialBuf);
                ////partialBuf.Release();
                //await Task.Delay(10);
                //Log.Info("send_part", i, parts.Count(), part.Length);
            }
        }

        public byte[] AddPartialRpc(ulong partialId, int partIndex, int totPartCount, byte[] payload)
        {
            if (!partialRpcDic.ContainsKey(partialId))
                partialRpcDic[partialId] = new byte[totPartCount][];
            partialRpcDic[partialId][partIndex] = payload;
            partialRpcTimeDic[partialId] = Fenix.Common.Utils.TimeUtil.GetTimeStampMS();
            Log.Info("recv_part", partIndex, totPartCount, payload.Length);
            if (partialRpcDic[partialId].Count(m => m != null) == totPartCount)
            {
                byte[] finalBytes = DataUtil.ConcatBytes(partialRpcDic[partialId]);
                partialRpcDic.TryRemove(partialId, out var _);
                partialRpcTimeDic.TryRemove(partialId, out var _);
                return finalBytes;
            }

            return null;
        }

        long lastTick = 0;

        public void Update()
        {
            var curTime = Fenix.Common.Utils.TimeUtil.GetTimeStampMS();

            if (curTime - lastTick < 5000)
                return;

            lastTick = curTime;

            CheckPeers(tcpPeers.Values);
            CheckPeers(kcpPeers.Values);
            CheckPeers(channelPeers.Values);
            
            foreach (var partialId in partialRpcTimeDic.Keys.ToArray())
            {
                var ts = partialRpcTimeDic[partialId];
                if (curTime - ts > 15000)
                {
                    Log.Info("CheckPartialRpc->timeout");
                    partialRpcDic.TryRemove(partialId, out var _);
                    partialRpcTimeDic.TryRemove(partialId, out var _);
                }
            }
        }

        void CheckPeers(ICollection<NetPeer> peers)
        {  
            var curTS = Fenix.Common.Utils.TimeUtil.GetTimeStampMS(); 
            foreach (var p in peers)
            {
                if (p.IsActive == false)
                {
                    Log.Info(string.Format("Remove: {0} {1} {2}", p.ConnId, p.RemoteAddress, p.netType));
                    this.Deregister(p);
                    continue;
                }
#if CLIENT
                if (curTS - p.lastTickTime >= Global.Config.HeartbeatIntervalMS * 3)
                {
                    this.PrintPeerInfo("SEKKIT");
                    Log.Info(string.Format("Timeout: {0} {1} {2}", p.ConnId, p.RemoteAddress, p.netType));
                    this.Deregister(p);
                }
#endif
            }
        }

        public void PrintPeerInfo(string header="")
        {
            if (header != "")
                Log.Info(header);

            Log.Info("#################################");
             
            foreach (var key in tcpPeers.Keys.Distinct().ToList())//var p in tcpPeers.Values)
            {
                var p = tcpPeers[key];
                Log.Info(string.Format("========PeerTCP({0}): {1} {2} {3} active:{4}", p.netType, p.ConnId, p.RemoteAddress.ToIPv4String(), p.LocalAddress.ToIPv4String(), p.IsActive)); 
            }

            foreach (var key in kcpPeers.Keys.Distinct().ToList())//var p in tcpPeers.Values)
            {
                var p = kcpPeers[key];
                Log.Info(string.Format("========PeerKCP({0}): {1} {2} {3} active:{4}", p.netType, p.ConnId, p.RemoteAddress.ToIPv4String(), p.LocalAddress.ToIPv4String(), p.IsActive));
            } 

            foreach (var key in channelPeers.Keys.Distinct().ToList())//var p in tcpPeers.Values)
            {
                var p = channelPeers[key];
                Log.Info(string.Format("========PeerCH({0}): {1} {2} {3} active:{4}", p.netType, p.ConnId, p.RemoteAddress.ToIPv4String(), p.LocalAddress.ToIPv4String(), p.IsActive));
            }

            Log.Info("#################################");
        }

        public void Destroy()
        {
            foreach (var p in tcpPeers.Values)
                Deregister(p);
            tcpPeers.Clear(); 
            foreach (var p in kcpPeers.Values)
                Deregister(p);
            kcpPeers.Clear(); 
            foreach (var p in channelPeers.Values)
                Deregister(p);
            channelPeers.Clear();  
            foreach (var kv in tcpServerDic) 
                kv.Value.Stop();
            tcpServerDic.Clear(); 
            foreach (var kv in kcpServerDic) 
                kv.Value.Stop();
            kcpServerDic.Clear(); 
            this.OnClose = null;
            this.OnConnect = null;
            this.OnException = null;
            this.OnReceive = null;
            //this.OnSend = null;
            //this.OnPeerLost = null;
            heartbeatTh?.Abort();
            heartbeatTh = null;
            Global.NetManager = null; 
        }
    }
}
