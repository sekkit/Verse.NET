
using DotNetty.Buffers;
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class TcpContainerServer: ITcpListener
    {
        public TcpSocketServer server;

        
        public event Action<IChannel> Close;

        public event Action<IChannel> Connect;

        public event Action<IChannel> Disconnect;

        public event Action<IChannel> Exception;

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

        public void OnException(IChannel channel)
        {
            Exception?.Invoke(channel);
        }

        public void OnReceive(IChannel channel, IByteBuffer buffer)
        {
            Console.WriteLine("server_rcv");
            Receive?.Invoke(channel, buffer);
            channel.WriteAndFlushAsync(buffer);
        }

        public async static Task<TcpContainerServer> Create(IPEndPoint ep)
        {
            return null;
        }
        
        public async static Task<TcpContainerServer> Create(string ip, int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = "0.0.0.0";
            channelConfig.Port = port;
            channelConfig.UseLibuv = true;
            var listener = new TcpContainerServer();
            var server = new TcpSocketServer();
            await server.Start(channelConfig, listener);
            return listener;
        }

        public void Send(NetPeer peer, byte[] bytes)
        {
            
        }
    }
}
