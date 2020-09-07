using DotNetty.Codecs;
using DotNetty.Common;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
#if !CLIENT 
using DotNetty.Transport.Libuv;
#endif
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DotNetty.TCP
{
    public class TcpSocketServer
    {
        IEventLoopGroup bossGroup;
        IEventLoopGroup workerGroup;
        volatile ServerBootstrap bootstrap;

        IChannel boundChannel;

        public bool Start(TcpChannelConfig channelConfig, ITcpListener listener)
        {
#if !CLIENT
            if (channelConfig.UseLibuv)
            {
                ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workerGroup = new MultithreadEventLoopGroup();
            }
#else
            bossGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();
#endif
            X509Certificate tlsCertificate = null;
            if (channelConfig.UseSSL)
            {
                tlsCertificate = new X509Certificate("dotnetty.com.pfx", "password");
            }

            try
            {
                bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);
#if !CLIENT
                if (channelConfig.UseLibuv)
                {
                    bootstrap.Channel<TcpServerChannel>();
                }
                else
                {
                    bootstrap.Channel<TcpServerSocketChannel>();
                }
#else
                bootstrap.Channel<TcpServerSocketChannel>();
#endif
                bootstrap
                  .Option(ChannelOption.SoBacklog, 128)
                  .ChildOption(ChannelOption.SoReuseaddr, true)
                  .ChildOption(ChannelOption.SoReuseport, false)
                  .ChildOption(ChannelOption.SoBroadcast, true)
                  .ChildOption(ChannelOption.SoKeepalive, true)
                  .ChildOption(ChannelOption.TcpNodelay, true)
                  .ChildOption(ChannelOption.SoSndbuf, 2048)
                  .ChildOption(ChannelOption.SoRcvbuf, 8096)
                  .Handler(new LoggingHandler());
//#if !CLIENT
//                if (channelConfig.UseLibuv)
//                {
                    bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        if (tlsCertificate != null)
                        {
                            pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
                        }

                        pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                        pipeline.AddLast("tcp-handler", new TcpChannelHandler(listener));
                    }));
//                }
//                else
//#endif
//                {
//                    bootstrap.ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
//                    {
//                        IChannelPipeline pipeline = channel.Pipeline;
//                        if (tlsCertificate != null)
//                        {
//                            pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
//                        }

//                        pipeline.AddLast(new LoggingHandler("SRV-CONN"));
//                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
//                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
//                        pipeline.AddLast("tcp-handler", new TcpChannelHandler(listener));
//                    }));
//                }
 
                var task = Task<IChannel>.Run(() => bootstrap.BindAsync(channelConfig.Address));
                task.Wait();
                boundChannel = task.Result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }

            if (boundChannel == null)
                return false;

            return true;
        }
    
        public async Task Shutdown()
        {
            await Task.WhenAll(
                    bossGroup?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            await boundChannel.CloseAsync();
        }
    }
}
