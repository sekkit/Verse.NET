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
        private Bootstrap bootstrap;
        protected TcpChannelConfig channelConfig;

        //private IChannel clientChannel;
        private MultithreadEventLoopGroup group;

        private TcpSocketClient()
        {

        }

        public static TcpSocketClient Instance = new TcpSocketClient();

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
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
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
            var task = Task<IChannel>.Run(() => bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(channelConfig.Address), channelConfig.Port)));
            task.Wait();
            var ch = task.Result;
            var chId = ch.Id.AsLongText(); 
            clientListenerDic[chId] = listener;
            return ch;
        } 
        
        public async void Shutdown()
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
            clientListenerDic.TryGetValue(chId, out var listener);
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
            if (clientListenerDic.TryGetValue(chId, out var listener))
                listener?.handleClose(channel);
        }

        public void handleException(IChannel channel, Exception ex)
        {
            var chId = channel.Id.AsLongText();
            if (clientListenerDic.TryGetValue(chId, out var listener))
                listener?.handleException(channel, ex);
        }

        public void StopChannel(IChannel channel)
        {
            var chId = channel.Id.AsLongText();
            if (clientListenerDic.TryGetValue(chId, out var listener))
                listener?.handleClose(channel);
            channel.CloseAsync();
        }

        protected ConcurrentDictionary<string, ITcpListener> clientListenerDic = new ConcurrentDictionary<string, ITcpListener>();
    }
}