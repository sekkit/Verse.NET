using System;
using DotNetty.KCP.Base;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.KCP.thread;
using fec;

namespace DotNetty.KCP
{
    public class ServerChannelHandler:ChannelHandlerAdapter
    {
        private readonly IChannelManager _channelManager;

        private readonly ChannelConfig _channelConfig ;

        private readonly IExecutorPool _executorPool;

        private readonly KcpListener _kcpListener;


        public ServerChannelHandler(IChannelManager channelManager, ChannelConfig channelConfig, IExecutorPool executorPool, KcpListener kcpListener)
        {
            _channelManager = channelManager;
            _channelConfig = channelConfig;
            _executorPool = executorPool;
            _kcpListener = kcpListener;
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception);
        }


        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            long last_ts = DateTime.Now.Ticks;
            var msg = (DatagramPacket) message;
            var channel = context.Channel;
            var ukcp = _channelManager.get(msg);
            var content = msg.Content;
            User user;
            if (ukcp != null)
            {
                user = ukcp.user();
                //每次收到消息重绑定地址
                user.RemoteAddress = msg.Sender;
                ukcp.read(content);
                //Console.WriteLine(string.Format("(LAGS1){0}", (DateTime.Now.Ticks - last_ts)/10000.0));
                return;
            }

            var messageExecutor = _executorPool.GetAutoMessageExecutor();
            KcpOutput kcpOutput = new KcpOutPutImp();

            ReedSolomon reedSolomon = null;
            if(_channelConfig.FecDataShardCount!=0&&_channelConfig.FecParityShardCount!=0){
                reedSolomon = ReedSolomon.create(_channelConfig.FecDataShardCount,_channelConfig.FecParityShardCount);
            }

            ukcp = new Ukcp(kcpOutput,_kcpListener,messageExecutor,reedSolomon,_channelConfig);

            user = new User(channel,msg.Sender,msg.Recipient);
            ukcp.user(user);

            _channelManager.New(msg.Sender,ukcp,msg);

            ukcp.connect();

            ukcp.read(content);

            var scheduleTask = new ScheduleTask(_channelManager, ukcp);
            //Console.WriteLine(ukcp.getInterval());
            KcpUntils.scheduleHashedWheel(scheduleTask, TimeSpan.FromMilliseconds(ukcp.getInterval()));
            
            Console.WriteLine(string.Format("(LAGS2){0}", (DateTime.Now.Ticks-last_ts)/10000.0));
        }


    }
}