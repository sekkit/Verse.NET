using DotNetty.Codecs;
using DotNetty.Common;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNetty.Buffers;


namespace DotNetty.TCP
{
    public class TcpSocketClient
    {
        private Bootstrap bootstrap;
        private IChannel clientChannel;
        private MultithreadEventLoopGroup group;
        
        public async Task Start(TcpChannelConfig channelConfig, ITcpListener listener)
        {  
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
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender2(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder2(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("tcp-handler", new TcpChannelHandler(listener));
                    }));

                clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(channelConfig.Address),
                        channelConfig.Port));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task SendAsync(byte[] bytes)
        {
            await clientChannel.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
        }
        
        public async void Shutdown()
        { 
            await clientChannel.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}