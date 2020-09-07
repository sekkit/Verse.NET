using DotNetty.Codecs; 
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets; 
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNetty.Buffers; 
using System.Collections.Concurrent;

namespace DotNetty.TCP
{ 
    public class TcpSocketClient : ITcpListener
    { 
        private volatile Bootstrap bootstrap;

        protected TcpChannelConfig channelConfig;
         
        private MultithreadEventLoopGroup group;

        public TcpSocketClient()
        {

        }  

        public bool init(TcpChannelConfig channelConfig)
        {
            this.channelConfig = channelConfig;

            group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            if (channelConfig.UseSSL)
            {
                cert = new X509Certificate2("dotnetty.com.pfx", "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }

            try
            {
                bootstrap = new Bootstrap();
                bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.SoReuseport, false)
                .Option(ChannelOption.SoSndbuf, 2048)
                .Option(ChannelOption.SoRcvbuf, 8096)
                .Option(ChannelOption.SoKeepalive, true);
//#if !CLIENT
//                if (channelConfig.UseLibuv)
//                {
                    bootstrap.Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (cert != null)
                        {
                            pipeline.AddLast("tls",
                                new TlsHandler(
                                    stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true),
                                    new ClientTlsSettings(targetHost)));
                        }

                        pipeline.AddLast(new LoggingHandler());
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                        pipeline.AddLast("tcp-handler", new TcpChannelHandler(this));
                    }));
//                }
//                else
//#endif
//                {
//                    bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
//                    {
//                        IChannelPipeline pipeline = channel.Pipeline;

//                        if (cert != null)
//                        {
//                            pipeline.AddLast("tls",
//                                new TlsHandler(
//                                    stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true),
//                                    new ClientTlsSettings(targetHost)));
//                        }

//                        pipeline.AddLast(new LoggingHandler());
//                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
//                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
//                        pipeline.AddLast("tcp-handler", new TcpChannelHandler(this));
//                    }));
//                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }

            return true;
        } 

        public IChannel Connect(IPEndPoint ep, ITcpListener listener)
        {

            try
            {
                //#if UNITY_5_3_OR_NEWER
                var task = Task<IChannel>.Run(() => bootstrap.ConnectAsync(ep));
                task.Wait();
                var ch = task.Result;
                //#else
                //            var ch = bootstrap.ConnectAsync(ep).Result;
                //#endif
                var chId = ch.Id.AsLongText();
                clientListenerDic[chId] = listener;
                return ch;
            }
            catch(Exception ex)
            {
                return null;
            } 
        } 
        
        public async Task Shutdown()
        {
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        public void handleConnect(IChannel channel)
        {
            var chId = channel.Id.AsLongText(); 
            if(clientListenerDic.TryGetValue(chId, out var listener))
                listener?.handleConnect(channel);
        }

        public void handleDisconnect(IChannel channel)
        {
            var chId = channel.Id.AsLongText(); 
            clientListenerDic.TryRemove(chId, out var listener);
            listener?.handleDisconnect(channel);
        }

        public void handleReceive(IChannel channel, IByteBuffer buffer)
        {
            var chId = channel.Id.AsLongText(); 
            if(clientListenerDic.TryGetValue(chId, out var listener))
                listener?.handleReceive(channel, buffer);
        }

        public void handleClose(IChannel channel)
        {
            var chId = channel.Id.AsLongText();
            if (clientListenerDic.TryRemove(chId, out var listener))
                listener?.handleClose(channel);
        }

        public void handleException(IChannel channel, Exception ex)
        {
            var chId = channel.Id.AsLongText();
            if (clientListenerDic.TryRemove(chId, out var listener))
                listener?.handleException(channel, ex);
        }

        public async Task StopChannel(IChannel channel)
        {
            var chId = channel.Id.AsLongText();
            clientListenerDic.TryRemove(chId, out var listener);
            //if (clientListenerDic.TryGetValue(chId, out var listener))
            //    listener?.handleClose(channel);
            //Task.Run(()=>channel.CloseAsync());
            await channel.CloseAsync();
        }

        protected ConcurrentDictionary<string, ITcpListener> clientListenerDic = new ConcurrentDictionary<string, ITcpListener>();
    }
}