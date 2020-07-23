using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.KCP;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Fenix.Common;
using Fenix.Common.Utils;



namespace Fenix
{
    public class NetManager
    {
        //protected ConcurrentDictionary<ulong, string> mChId2ChName = new ConcurrentDictionary<ulong, string>();
        
        //protected ConcurrentDictionary<string, IChannel> mChName2Ch = new ConcurrentDictionary<string, IChannel>();

        //protected ConcurrentDictionary<string, Ukcp> mAddr2Ukcp = new ConcurrentDictionary<string, Ukcp>();

        protected ConcurrentDictionary<uint, NetPeer> mPeers = new ConcurrentDictionary<uint, NetPeer>();
        
        public static NetManager Instance = new NetManager();

        public void RegisterChannel(IChannel channel)
        {
            //var channelId = channel.Id;
            //var channelName = channelId.AsLongText(); 
            //mChName2Ch[channelName] = channel;

            var addr = channel.RemoteAddress.ToString();

            var id = Global.IdManager.GetHostId(addr);

            //mChId2ChName[id] = channelName;

            var peer = NetPeer.Create(id, channel);
            mPeers[peer.ConnId] = peer;
        }

        public void DeregisterChannel(IChannel ch)
        {
            var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString()); 
            this.mPeers.TryRemove(id, out NetPeer peer);
            if(id != 0 && peer != null)
                Global.IdManager.RemoveHostId(peer.ConnId);

            //this.mChName2Ch.TryRemove(ch.Id.AsLongText(), out var c);
            //this.mChId2ChName.TryRemove(id, out var cn);
        }

        public void ChangePeerId(uint oldId, uint newId)
        { 
            var peer = mPeers[oldId]; 
            peer.ConnId = newId;
            mPeers.TryRemove(oldId, out var _);
            mPeers[newId] = peer;
            Global.IdManager.RegisterAddress(newId, peer.RemoteAddress.ToString());
        }

        //kcp目前不支持epoll/kqueue/IOCP，所以只在客户端上用用
        public void RegisterKcp(Ukcp ukcp)
        {
            //如果进来的连接，没有id，则根据地址生成一个
            //int conv = ukcp.getConv();
            var ep = ukcp.user().RemoteAddress;
            //var cid = ukcp.user().Channel.Id.AsLongText();

            var addr = ep.ToString();

            var id = Global.IdManager.GetHostId(addr);
            if(id == 0)
            {
                //生成一个
                id = Basic.GenID32FromName(addr);
            }

            //this.mAddr2Ukcp[addr] = ukcp;

            var peer = NetPeer.Create(id, ukcp);
            mPeers[peer.ConnId] = peer;

            Log.Info(string.Format("Incoming KCP id: {0}", id));
        }  

        public void DeregisterKcp(Ukcp ukcp)
        {
            var ep = ukcp.user().RemoteAddress;
            var addr = ep.ToString();

            //this.mAddr2Ukcp.TryRemove(addr, out var _);
            var id = Global.IdManager.GetHostId(addr);
            if(id == 0)
            {
                id = Basic.GenID32FromName(addr);
            }
            mPeers.TryRemove(id, out var _);
        }

        public void Deregister(NetPeer peer)
        {
            var ep = peer.RemoteAddress;// ukcp.user().RemoteAddress;
            var addr = ep.ToString();

            //this.mAddr2Ukcp.TryRemove(addr, out var _);
            var id = Global.IdManager.GetHostId(addr);
            if (id == 0)
            {
                id = Basic.GenID32FromName(addr);
            }
            mPeers.TryRemove(id, out var _);
        }

        public void RemovePeerId(uint connId)
        {
            mPeers.TryRemove(connId, out var peer);
            Global.IdManager.RemoveHostId(connId);
        }

        public NetPeer GetPeer(IChannel ch)
        {
            var id = Global.IdManager.GetHostId(ch.RemoteAddress.ToString());
            return mPeers[id];
        }

        public NetPeer GetPeer(Ukcp ukcp)
        {
            var ep = ukcp.user().RemoteAddress; 

            var addr = ep.ToString();

            var id = Global.IdManager.GetHostId(addr);
            if (id == 0)
            {
                //生成一个
                id = Basic.GenID32FromName(addr);
            }

            return mPeers[id];
        }

        public NetPeer GetPeerById(uint peerId)
        {
            NetPeer peer;
            if (mPeers.TryGetValue(peerId, out peer))
                return peer;
            return null;
        }

        public NetPeer CreatePeer(uint remoteHostId, IPEndPoint addr, NetworkType netType)
        { 
            var peer = GetPeerById(remoteHostId);
            if (peer != null)
                return peer;
            peer = NetPeer.Create(remoteHostId, addr, netType);
            if (peer == null)
                return null;
            mPeers[peer.ConnId] = peer;
            //Global.IdManager.RegisterAddress()
            return peer; 
        }

        //peer connects two hosts(processes)
        public NetPeer CreatePeer(string ip, int port, NetworkType netType)
        {
//#if !CLIENT
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            var addr = ep.ToString();

            var cid = Global.IdManager.GetHostId(addr);
            if (cid != 0)
                return NetManager.Instance.GetPeerById(cid);
//#endif

            var peer = NetPeer.Create(ep, netType);
            if (peer == null)
                return null;
            mPeers[peer.ConnId] = peer;
            return peer;
        }
    }
}
