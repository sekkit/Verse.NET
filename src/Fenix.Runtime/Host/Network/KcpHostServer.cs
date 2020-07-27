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
    public class KcpHostServer : KcpListener
    {
        public event Action<Ukcp> OnConnect;

        public event Action<Ukcp, IByteBuffer> OnReceive; 

        public event Action<Ukcp, Exception> OnException;

        public event Action<Ukcp> OnClose;

        protected KcpServer server; 

        public static KcpHostServer Create(IPEndPoint ep)
        {
            KcpHostServer listener = new KcpHostServer();

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
            //    var objs = (object[])obj;
            //OnReceive?.Invoke(ukcp, byteBuf);
            //   OnReceive?.Invoke((Ukcp)objs[1], (IByteBuffer)objs[0]);
            //}, new object[] { byteBuf.Retain(), ukcp });
            OnReceive?.Invoke(ukcp, byteBuf);
        }
         
        public void handleException(Ukcp ukcp, Exception ex)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{
            //    var objs = (object[])obj;
            //    OnException?.Invoke((Ukcp)objs[0], (Exception)objs[1]);
            //}, new object[] { ukcp, ex});
            OnException?.Invoke(ukcp, ex);
        }

        public void handleClose(Ukcp ukcp)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{
            //    OnClose?.Invoke((Ukcp)obj);
            //}, ukcp);

            OnClose?.Invoke(ukcp);

            Log.Info(Snmp.snmp.ToString());
            Snmp.snmp = new Snmp();
        }

        public void handleConnect(Ukcp ukcp)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{ 
            //    OnConnect?.Invoke((Ukcp)obj);
            //}, ukcp);
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
