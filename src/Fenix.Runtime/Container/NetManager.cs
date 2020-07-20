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

        protected Dictionary<uint, NetPeer> mPeers = new Dictionary<uint, NetPeer>();
        
        //protected Dictionary<uint, TcpContainerClient> mTcpClientDic { get; set; }
        
        //protected Dictionary<uint, KcpContainerClient> mKcpClientDic { get; set; }
        
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
            this.mPeers.Remove(id);

            this.mChName2Ch.TryRemove(ch.Id.AsLongText(), out var c);
            this.mChId2ChName.TryRemove(id, out var cn);
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
            var task = Task.Run(() => NetPeer.Create(remoteContainerId, true));
            task.Wait();
            peer = task.Result;
            mPeers[peer.ConnId] = peer;
            return peer; 
        }
    }
}
