using System;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.KCP.thread;

namespace DotNetty.KCP
{
    public class ClientChannelHandler:ChannelHandlerAdapter
    {
        private readonly IChannelManager _channelManager;

        private readonly ChannelConfig _channelConfig;


        public ClientChannelHandler(IChannelManager channelManager,ChannelConfig channelConfig)
        {
            this._channelManager = channelManager;
            this._channelConfig = channelConfig;
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
#if !UNITY_5_3_OR_NEWER
            Console.WriteLine(exception.ToString());
#else
            UnityEngine.Debug.Log(exception.ToString());
#endif
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = (DatagramPacket) message;
            var ukcp = _channelManager.get(msg);
            ukcp?.read(msg.Content);
        }
    }
}