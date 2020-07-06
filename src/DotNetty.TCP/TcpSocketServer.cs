using DotNetty.Codecs;
using DotNetty.Common;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DotNetty.TCP
{
    public class TcpSocketServer
    {
        IEventLoopGroup bossGroup;
        IEventLoopGroup workerGroup;

        IChannel boundChannel;

        public async Task Start(TcpChannelConfig channelConfig, ITcpListener listener)
        {
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

            X509Certificate tlsCertificate = null;
            if (channelConfig.UseSSL)
            {
                tlsCertificate = new X509Certificate("dotnetty.com.pfx", "password");
            }

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);

                if (channelConfig.UseLibuv)
                {
                    bootstrap.Channel<TcpServerChannel>();
                }
                else
                {
                    bootstrap.Channel<TcpServerSocketChannel>();
                }

                bootstrap
                  .Option(ChannelOption.SoBacklog, 8192)
                  .Handler(new LoggingHandler())
                  .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
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

                boundChannel = await bootstrap.BindAsync(channelConfig.Port);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    
        public async void Shutdown()
        {
            await Task.WhenAll(
                    bossGroup?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            await boundChannel.CloseAsync();
        }
    }
}
