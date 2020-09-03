using DotNetty.Buffers;
using DotNetty.KCP;
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

        public static volatile TcpSocketClient client;

        protected IChannel clientChannel;

        public bool IsActive => clientChannel.Active;

        private bool IsAlive = true;

        public IPEndPoint RemoteAddress => (IPEndPoint)clientChannel.RemoteAddress; 

        public IPEndPoint LocalAddress => (IPEndPoint)clientChannel.LocalAddress;

        public string ChannelId => clientChannel.Id.AsLongText();

        static object lockObj = new object();

        public bool Init(TcpChannelConfig channelConfig, IPEndPoint ep)
        {  
            if(client == null)
            {
                lock (lockObj)
                {
                    if (client == null)
                    {
                        client = new TcpSocketClient();
                        if (!client.init(channelConfig))
                        {
                            client = null;
                            return false;
                        }
                    }
                }
            }

            this.clientChannel = client.Connect(ep, this);
            if (this.clientChannel == null)
                return false;
            return true;
        }

        public void handleConnect(IChannel channel)
        {
            OneThreadSynchronizationContext.Instance.Post((obj) =>
            {
                OnConnect?.Invoke((IChannel)obj);
            }, channel);
        }

        public void handleDisconnect(IChannel channel)
        { 
            OneThreadSynchronizationContext.Instance.Post((obj) =>
            {
                OnDisconnect?.Invoke((IChannel)obj);
            }, channel);
        }

        void ITcpListener.handleReceive(IChannel channel, IByteBuffer buffer)
        {
            OneThreadSynchronizationContext.Instance.Post((obj) =>
            {
                var objs = (object[])obj;
                OnReceive?.Invoke((IChannel)objs[0], (IByteBuffer)objs[1]);
            }, new object[] { channel, buffer });

            //OnReceive?.Invoke(channel, buffer); 
        }

        void ITcpListener.handleClose(IChannel channel)
        {
            if (IsAlive == false)
                return;

            IsAlive = false;

            OneThreadSynchronizationContext.Instance.Post((obj) =>
            {
                OnClose?.Invoke((IChannel)obj);
            }, channel);

            OnClose?.Invoke(channel); 
        }

        void ITcpListener.handleException(IChannel channel, Exception ex)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{
            //    OnDisconnect?.Invoke((IChannel)obj);
            //}, channel);

            OnException?.Invoke(channel, ex);  
        }

        public static TcpHostClient Create(IPEndPoint ep)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = ep;
            //channelConfig.Address = ep.Address.ToIPv4String();
            //channelConfig.Port = ep.Port;
#if !CLIENT
            channelConfig.UseLibuv = false;
#endif

            var obj = new TcpHostClient();
            if (!obj.Init(channelConfig, ep))
                return null;            
            return obj;
        }

        public async Task SendAsync(byte[] bytes)
        {
            await clientChannel?.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
        }

        //public void Send(byte[] bytes)
        //{
        //    clientChannel?.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
    
        //    //SendAsync(bytes);
        //    //    /*
        //    //var task = Task.Run(()=> SendAsync(bytes));
        //    //task.Wait();
        //    //    */
        //}

        public void Send(IByteBuffer buffer)
        {
            clientChannel?.WriteAndFlushAsync(buffer); 
        }

        public async Task Stop()
        {
            await client.StopChannel(clientChannel); 
        }
    }
}
