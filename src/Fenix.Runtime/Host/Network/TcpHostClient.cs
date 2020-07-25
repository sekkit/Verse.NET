using DotNetty.Buffers; 
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using Fenix.Common;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class TcpHostClient: ITcpListener
    {
        public event Action<IChannel> OnClose;

        public event Action<IChannel> OnConnect;

        public event Action<IChannel> OnDisconnect;

        public event Action<IChannel, Exception> OnException;

        public event Action<IChannel, IByteBuffer> OnReceive;

        public static TcpSocketClient client;

        protected IChannel clientChannel;

        public bool IsActive => clientChannel.Active;

        private bool IsAlive = true;

        public IPEndPoint RemoteAddress => (IPEndPoint)clientChannel.RemoteAddress; 

        public IPEndPoint LocalAddress => (IPEndPoint)clientChannel.LocalAddress;

        public string ChannelId => clientChannel.Id.AsLongText();

        public void handleConnect(IChannel channel)
        { 
            OnConnect?.Invoke(channel); 
        }

        public void handleDisconnect(IChannel channel)
        {
            OnDisconnect?.Invoke(channel);
        }

        void ITcpListener.handleReceive(IChannel channel, IByteBuffer buffer)
        {
            OnReceive?.Invoke(channel, buffer); 
        }

        void ITcpListener.handleClose(IChannel channel)
        {
            if (IsAlive == false)
                return;

            IsAlive = false;

            OnClose?.Invoke(channel); 
        }

        void ITcpListener.handleException(IChannel channel, Exception ex)
        {
            OnException?.Invoke(channel, ex);  
        }

        public static TcpHostClient Create(IPEndPoint ep)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = ep.Address.ToString();
            channelConfig.Port = ep.Port;
#if !UNITY_5_3_OR_NEWER
            channelConfig.UseLibuv = false;
#endif
            if (client==null)
            {
                client = TcpSocketClient.Instance;
                if (!client.init(channelConfig))
                {
                    client = null;
                    return null;
                }
            }

            var listener = new TcpHostClient();
            listener.clientChannel = client.Connect(ep, listener);
            if (listener.clientChannel == null)
                return null;
            return listener;
        }

        public async Task SendAsync(byte[] bytes)
        {
            await clientChannel?.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
        }

        public void Send(byte[] bytes)
        {
            clientChannel?.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
            //SendAsync(bytes);
            //SendAsync(bytes);
            //    /*
            //var task = Task.Run(()=> SendAsync(bytes));
            //task.Wait();
            //    */
        }

        public void Stop()
        {
            client.StopChannel(clientChannel); 
        }
    }
}
