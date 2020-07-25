using System;
using System.Collections.Generic;
using System.Text;   
using DotNetty.Buffers;
using DotNetty.Transport.Channels;


namespace DotNetty.TCP
{

    public class TcpChannelHandler : ChannelHandlerAdapter
    {
        protected ITcpListener listener;

        public TcpChannelHandler(ITcpListener _listener)
        {
            this.listener = _listener;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            this.listener?.handleConnect(context.Channel);
            
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context); 
            //this.listener?.handleDisconnect(context.Channel);
            this.listener?.handleClose(context.Channel);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //var buffer = message as IByteBuffer;
            //if (buffer != null)
            //{
            //    Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
            //}
            //context.WriteAsync(message);
            //long last_ts = DateTime.Now.Ticks;
            this.listener?.handleReceive(context.Channel, message as IByteBuffer);
            //Console.WriteLine("LAG3:"+(DateTime.Now.Ticks-last_ts)/10000.0);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception.Message); 
            this.listener?.handleException(context.Channel, exception);
            context.CloseAsync();
        }
    }
}
