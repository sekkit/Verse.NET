
using DotNetty.Buffers;
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using Fenix.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class TcpHostServer: ITcpListener
    {
        public TcpSocketServer server;
        
        public event Action<IChannel> OnClose;

        public event Action<IChannel> OnConnect;

        public event Action<IChannel> OnDisconnect;

        public event Action<IChannel, Exception> OnException;

        public event Action<IChannel, IByteBuffer> OnReceive; 
        
        public void handleClose(IChannel channel)
        {
            OnClose?.Invoke(channel);
        }

        void ITcpListener.handleConnect(IChannel channel)
        {
            OnConnect?.Invoke(channel); 
        }

        void ITcpListener.handleDisconnect(IChannel channel)
        {
            OnDisconnect?.Invoke(channel);
        }

        void ITcpListener.handleException(IChannel channel, Exception ex)
        {
            OnException?.Invoke(channel, ex);
        }

        void ITcpListener.handleReceive(IChannel channel, IByteBuffer buffer)
        {
            OnReceive?.Invoke(channel, buffer); 
        }

        public static TcpHostServer Create(IPEndPoint ep)
        {
            return Create(ep.Address.ToString(), ep.Port);
        }
        
        public static TcpHostServer Create(string ip, int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = ip;// "0.0.0.0";
            channelConfig.Port = port;
#if !UNITY_5_3_OR_NEWER
            channelConfig.UseLibuv = true;
#endif
            var listener = new TcpHostServer();
            var server = new TcpSocketServer();
            if (!server.Start(channelConfig, listener))
                return null;
            return listener;
        } 

        //public void Send(NetPeer peer, byte[] bytes)
        //{
        //    peer.Send(bytes);
        //}
    }
}
