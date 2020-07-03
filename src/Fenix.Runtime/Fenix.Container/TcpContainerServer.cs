
using DotNetty.Buffers;
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class TcpContainerServer: ITcpServerListener
    {
        public TcpSocketServer server;

        public void OnClose(IChannel channel)
        {
            
        }

        public void OnConnect(IChannel channel)
        {
            
        }

        public void OnDisconnect(IChannel channel)
        {
            
        }

        public void OnException(IChannel channel)
        {
            
        }

        public void OnReceive(IChannel channel, IByteBuffer buffer)
        {
            
        }

        public async void Setup(int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = "0.0.0.0";
            channelConfig.Port = port;
            channelConfig.UseLibuv = true;

            server = new TcpSocketServer();
            await server.Start(channelConfig, this);
        }

        public void Send(NetPeer peer, byte[] bytes)
        {
            
        }
    }
}
