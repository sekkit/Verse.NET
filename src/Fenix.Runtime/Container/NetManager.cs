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
        protected ConcurrentDictionary<ulong, string> Id2ChannelName = new ConcurrentDictionary<ulong, string>();
        
        protected ConcurrentDictionary<string, IChannel> ChannelName2Channel = new ConcurrentDictionary<string, IChannel>();
        
        protected Dictionary<uint, NetPeer> peers { get; set; }
        
        protected Dictionary<uint, TcpContainerClient> tcpClientDic { get; set; }
        
        protected Dictionary<uint, KcpContainerClient> kcpClientDic { get; set; }
        
        public static NetManager Instance = new NetManager();

        public void RegisterChannel(IChannel channel)
        { 
            var channelId = channel.Id;
            var channelName = channelId.AsLongText(); 
            ChannelName2Channel[channelName] = channel;
        }

        public void DeregisterChannel(ulong id, IChannel channel)
        {
            
        }
    }
}
