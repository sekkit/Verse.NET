using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;

namespace Fenix
{
    public class NetManager
    {
        protected ConcurrentDictionary<ulong, string> mId2ChName = new ConcurrentDictionary<ulong, string>();
        
        protected ConcurrentDictionary<string, IChannel> mChName2Ch = new ConcurrentDictionary<string, IChannel>();
        
        protected Dictionary<uint, NetPeer> mPeers { get; set; }
        
        protected Dictionary<uint, TcpContainerClient> mTcpClientDic { get; set; }
        
        protected Dictionary<uint, KcpContainerClient> mKcpClientDic { get; set; }
        
        public static NetManager Instance = new NetManager();

        public void RegisterChannel(IChannel channel)
        {
            var channelId = channel.Id;
            var channelName = channelId.AsLongText(); 
            mChName2Ch[channelName] = channel;

            var addr = channel.RemoteAddress.ToString();

            var Id = Global.IdManager.GetContainerId(addr);
            
            var peer = NetPeer.Create(Id, channel);
            mPeers[peer.ConnId] = peer;
        }

        public void DeregisterChannel(ulong connId, IChannel channel)
        {
            
        }

        public NetPeer GetPeer(IChannel ch)
        {
            
        }
    }
}
