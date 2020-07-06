using DotNetty.Buffers; 
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class TcpContainerClient: ITcpListener
    {
        public event Action<IChannel> Close;

        public event Action<IChannel> Connect;

        public event Action<IChannel> Disconnect;

        public event Action<IChannel> Exception;

        public event Action<IChannel, IByteBuffer> Receive;

        public TcpSocketClient client;
        
        public void OnConnect(IChannel channel)
        {
            Connect?.Invoke(channel);
        }

        public void OnDisconnect(IChannel channel)
        {
            Disconnect?.Invoke(channel);
        }

        void ITcpListener.OnReceive(IChannel channel, IByteBuffer buffer)
        {
            Console.WriteLine("recv");
            channel.WriteAndFlushAsync(buffer);
            Receive?.Invoke(channel, buffer);
        }

        void ITcpListener.OnClose(IChannel channel)
        {
            Close?.Invoke(channel);
        }

        void ITcpListener.OnException(IChannel channel)
        {
            Exception?.Invoke(channel);
        }

        public async static Task<TcpContainerClient> Create(string ip, int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = "127.0.0.1";
            channelConfig.Port = port;
            channelConfig.UseLibuv = true;

            var listener = new TcpContainerClient();
            listener.client = new TcpSocketClient();
            await listener.client.Start(channelConfig, listener);
            return listener;
        }

        public void Send(byte[] bytes)
        {
            this.client.SendAsync(bytes);
        }
 
    }
}
