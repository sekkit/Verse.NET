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

        protected volatile KcpServer server; 

        public static KcpHostServer Create(IPEndPoint ep)
        {
            ChannelConfig channelConfig = new ChannelConfig(); 
            channelConfig.Crc32Check = false;
            channelConfig.initNodelay(true, 10, 2, true);
            channelConfig.Sndwnd = 512;
            channelConfig.Rcvwnd = 512;
            channelConfig.Mtu = 512; 
            channelConfig.FecDataShardCount = 0;
            channelConfig.FecParityShardCount = 0;
            channelConfig.AckNoDelay = true;
            channelConfig.TimeoutMillis = 10000;
            //channelConfig.Conv = 55;
            ////AutoSetConv = true;
            channelConfig.UseConvChannel = false;  
            KcpHostServer listener = new KcpHostServer();
            if (!listener.Init(channelConfig, ep))
                return null;
            return listener;
        }

        public bool Init(ChannelConfig channelConfig, IPEndPoint ep)
        {  
            server = new KcpServer();
            server.init(Environment.ProcessorCount, this, channelConfig, ep.Port); 
            return true;
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
         
        public void handleException(Exception ex, Ukcp ukcp)
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

        public void onConnected(Ukcp ukcp)
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
