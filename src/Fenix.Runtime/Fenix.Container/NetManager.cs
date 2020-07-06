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
        protected IChannelGroup ChannelGroup = new DefaultChannelGroup();
        protected ConcurrentDictionary<ulong, string> Id2ChannelName = new ConcurrentDictionary<ulong, string>();
        protected ConcurrentDictionary<string, IChannelId> ChannelName2Channel = new ConcurrentDictionary<string, IChannelId>();
        
        public static NetManager Instance = new NetManager();

        public void RegisterChannel(ulong id, IChannel channel)
        {
            ChannelGroup.Add(channel);
            var channelId = channel.Id;
            var channelName = channelId.AsLongText();
            Id2ChannelName[id] = channelId.AsLongText();
            ChannelName2Channel[channelName] = channelId;
        }

        public void DeregisterChannel(ulong id, IChannel channel)
        {
            
        }
    }
}
