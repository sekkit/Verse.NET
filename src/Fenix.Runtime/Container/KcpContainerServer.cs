using DotNetty.Buffers;
using DotNetty.KCP; 
using fec;
using Fenix.Common;
using Fenix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Fenix
{
    public class KcpContainerServer : KcpListener
    {
        public event Action<Ukcp> OnConnect;

        public event Action<Ukcp, IByteBuffer> OnReceive; 

        public event Action<Ukcp, Exception> OnException;

        public event Action<Ukcp> OnClose;

        protected KcpServer server; 
        

        public static KcpContainerServer Create(IPEndPoint ep)
        {
            KcpContainerServer listener = new KcpContainerServer();

            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.KcpTag = false;
            channelConfig.Crc32Check = true;
            channelConfig.initNodelay(true, 0, 2, true);
            channelConfig.Sndwnd = 512;
            channelConfig.Rcvwnd = 512;
            channelConfig.Mtu = 512; 
            channelConfig.FecDataShardCount = 3;
            channelConfig.FecParityShardCount = 1;
            channelConfig.AckNoDelay = true;
            channelConfig.TimeoutMillis = 10000;
            //channelConfig.Conv = 55;
            ////AutoSetConv = true;
            channelConfig.UseConvChannel = false; 
            listener.server = new KcpServer();
            listener.server.init(Environment.ProcessorCount, listener, channelConfig, ep.Port);
            
            return listener;
        }

        public void handleReceive(IByteBuffer byteBuf, Ukcp ukcp)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{ 
            //    OnReceive?.Invoke(byteBuf, ukcp, protocolType);
           
            OnReceive?.Invoke(ukcp, byteBuf); 
            
            //}, null);

            //short curCount = byteBuf.GetShort(byteBuf.ReaderIndex);
            //Console.WriteLine(Thread.CurrentThread.Name + " 收到消息 " + curCount);
            //ukcp.writeKcpMessage(byteBuf);
            //if (curCount == -1)
            //{
            //    ukcp.notifyCloseEvent();
            //}
        }

        public void handleException(Ukcp ukcp, Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            OnException?.Invoke(ukcp, ex); 
        }

        public void handleClose(Ukcp ukcp)
        { 
            OnClose?.Invoke(ukcp); 

            Console.WriteLine(Snmp.snmp.ToString());
            Snmp.snmp = new Snmp();
        }

        public void handleConnect(Ukcp ukcp)
        { 
            OnConnect?.Invoke(ukcp); 
        }

        //public void Send(byte[] bytes)
        //{
        //    IByteBuffer buf = Unpooled.WrappedBuffer(bytes);
        //    int dataLen = buf.ReadableBytes;
        //    _ukcp.writeKcpMessage(buf);
        //}
    }
}
