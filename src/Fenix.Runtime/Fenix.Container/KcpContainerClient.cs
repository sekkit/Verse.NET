using DotNetty.Buffers;
using DotNetty.Common.Utilities;
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
    public class KcpContainerClient : KcpListener
    {
        public event Action<byte[], Ukcp> OnReceive;

        public event Action<Exception, Ukcp> OnException;

        public event Action<Ukcp> OnClose;

        protected KcpClient client;

        private static Ukcp _ukcp;

        public static KcpContainerClient Create(string ip, int port)
        {
            KcpContainerClient listener = new KcpContainerClient();

            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.KcpTag = false;
            channelConfig.Crc32Check = true;
            channelConfig.initNodelay(true, 40, 2, true);
            channelConfig.Sndwnd = 512;
            channelConfig.Rcvwnd = 512;
            channelConfig.Mtu = 512;
            channelConfig.FecDataShardCount = 3;
            channelConfig.FecParityShardCount = 1;
            channelConfig.AckNoDelay = true;
            channelConfig.Conv = 10;//.AutoSetConv = true;
            channelConfig.UseConvChannel = true; 
            listener.client = new KcpClient();
            listener.client.init(channelConfig);

            EndPoint remoteAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            _ukcp = listener.client.connect(remoteAddress, channelConfig, listener);

            return listener;
        } 
       

        public void handleReceive(IByteBuffer byteBuf, Ukcp ukcp)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{
            //byte[] bytes = new byte[byteBuf.ReadableBytes];
            //byteBuf.GetBytes(byteBuf.ReaderIndex, bytes);
            //OnReceive?.Invoke(bytes, ukcp, protocolType);
            OnReceive?.Invoke(byteBuf.ToArray(), ukcp);
                //OnReceive?.Invoke(byteBuf, ukcp, protocolType);
            //}, null);

            //short curCount = byteBuf.GetShort(byteBuf.ReaderIndex);
            //Console.WriteLine(Thread.CurrentThread.Name + " 收到消息 " + curCount);
            //ukcp.writeKcpMessage(byteBuf);
            //if (curCount == -1)
            //{
            //    ukcp.notifyCloseEvent();
            //}
        }

        public void handleException(Exception ex, Ukcp ukcp)
        {
            Console.WriteLine(ex.StackTrace);
            OnException?.Invoke(ex, ukcp);
        }

        public void handleClose(Ukcp ukcp)
        {
            OnClose?.Invoke(ukcp);

            Console.WriteLine(Snmp.snmp.ToString());
            Snmp.snmp = new Snmp();
        }

        public void Send(byte[] bytes)
        {
            IByteBuffer buf = Unpooled.WrappedBuffer(bytes);
            int dataLen = buf.ReadableBytes;
            _ukcp.writeMessage(buf);
        }
    }
}
