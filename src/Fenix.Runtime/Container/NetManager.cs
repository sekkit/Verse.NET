using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Serilog.Sinks.File;

namespace Fenix
{
    public class NetManager
    {
        protected ConcurrentDictionary<ulong, string> mChId2ChName = new ConcurrentDictionary<ulong, string>();
        
        protected ConcurrentDictionary<string, IChannel> mChName2Ch = new ConcurrentDictionary<string, IChannel>();

        protected ConcurrentDictionary<uint, NetPeer> mPeers = new ConcurrentDictionary<uint, NetPeer>();
        
        public static NetManager Instance = new NetManager();

        public void RegisterChannel(IChannel channel)
        {
            var channelId = channel.Id;
            var channelName = channelId.AsLongText(); 
            mChName2Ch[channelName] = channel;

            var addr = channel.RemoteAddress.ToString();

            var id = Global.IdManager.GetContainerId(addr);

            mChId2ChName[id] = channelName;

            var peer = NetPeer.Create(id, channel);
            mPeers[peer.ConnId] = peer;
        }

        public void DeregisterChannel(IChannel ch)
        {
            var id = Global.IdManager.GetContainerId(ch.RemoteAddress.ToString()); 
            this.mPeers.TryRemove(id, out NetPeer peer);
            if(id != 0 && peer != null)
                Global.IdManager.RemoveContainerId(peer.ConnId);

            this.mChName2Ch.TryRemove(ch.Id.AsLongText(), out var c);
            this.mChId2ChName.TryRemove(id, out var cn);
        }

        public void RemovePeerId(uint connId)
        {
            mPeers.TryRemove(connId, out var peer);
            Global.IdManager.RemoveContainerId(connId);
        }

        public NetPeer GetPeer(IChannel ch)
        {
            var id = Global.IdManager.GetContainerId(ch.RemoteAddress.ToString());
            return mPeers[id];
        }

        public NetPeer GetPeerById(uint peerId)
        {
            NetPeer peer;
            if (mPeers.TryGetValue(peerId, out peer))
                return peer;
            return null;
        }

        public NetPeer CreatePeer(uint remoteContainerId)
        { 
            var peer = GetPeerById(remoteContainerId);
            if (peer != null)
                return peer;
            peer = NetPeer.Create(remoteContainerId, true);
            if (peer == null)
                return null;
            mPeers[peer.ConnId] = peer; 
            return peer; 
        }
    }
}
