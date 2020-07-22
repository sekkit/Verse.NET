
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
        
        public event Action<IChannel> Close;

        public event Action<IChannel> Connect;

        public event Action<IChannel> Disconnect;

        public event Action<IChannel, Exception> Exception;

        public event Action<IChannel, IByteBuffer> Receive;
        
        public void OnClose(IChannel channel)
        {
            Close?.Invoke(channel);
        }

        public void OnConnect(IChannel channel)
        { 
            Connect?.Invoke(channel); 
        }

        public void OnDisconnect(IChannel channel)
        {
            Disconnect?.Invoke(channel);
        }

        public void OnException(IChannel channel, Exception ex)
        {
            Exception?.Invoke(channel, ex);
        }

        public void OnReceive(IChannel channel, IByteBuffer buffer)
        {
            Receive?.Invoke(channel, buffer); 
        }

        public static TcpHostServer Create(IPEndPoint ep)
        {
            return Create(ep.Address.ToString(), ep.Port);
        }
        
        public static TcpHostServer Create(string ip, int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = "0.0.0.0";
            channelConfig.Port = port;
            channelConfig.UseLibuv = true;
            var listener = new TcpHostServer();
            var server = new TcpSocketServer();
            if (!server.Start(channelConfig, listener))
                return null;
            return listener;
        }

        public void Send(NetPeer peer, byte[] bytes)
        {
            
        }
    }
}
