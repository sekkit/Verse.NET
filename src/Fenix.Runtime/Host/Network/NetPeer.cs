
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.KCP;
using DotNetty.Transport.Channels;
using Fenix.Common;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class NetPeer
    { 
        public ulong ConnId { get; set; }

        public Ukcp kcpChannel { get; set; }
        
        public IChannel tcpChannel { get; set; }

        protected TcpHostClient tcpClient { get; set; }

        protected KcpHostClient kcpClient { get; set; }

        public event Action<NetPeer, IByteBuffer> OnReceive;

        public event Action<NetPeer> OnClose;

        public event Action<NetPeer, Exception> OnException;

        public event Action<NetPeer, IByteBuffer> OnSend;

        public NetworkType netType;

        public double lastTickTime = 0;

        public bool IsActive
        {
            get
            {
                if (this.tcpChannel != null)
                    return tcpChannel.Active;
                if (this.kcpChannel != null)
                    return kcpChannel.isActive();
                if (this.kcpClient != null)
                    return kcpClient.IsActive;
                if (this.tcpClient != null)
                    return this.tcpClient.IsActive;
                return false;
            }
        }

        public bool IsAlive = true;

        public bool IsRemoteClient
        {
            get
            {
                if (tcpChannel != null)
                    return true;
                if (tcpClient != null)
                    return false;
                if (kcpChannel != null)
                    return true; 
                return false;
            }
        }
        
        protected NetPeer()
        {
            lastTickTime = Common.Utils.TimeUtil.GetTimeStampMS();
        }

        public IPEndPoint RemoteAddress
        {
            get
            {
                if (this.kcpClient != null)
                    return kcpClient.RemoteAddress;
                if (this.tcpClient != null)
                    return tcpClient.RemoteAddress;
                if (this.kcpChannel != null)
                    return (IPEndPoint)kcpChannel.user().RemoteAddress;
                if (this.tcpChannel != null)
                    return (IPEndPoint)tcpChannel.RemoteAddress;
                return null;
            }
        }

        public IPEndPoint LocalAddress
        {
            get
            {
                if (this.kcpClient != null)
                    return kcpClient.LocalAddress;
                if (this.tcpClient != null)
                    return tcpClient.LocalAddress;
                if (this.kcpChannel != null)
                    return (IPEndPoint)kcpChannel.user().LocalAddress;
                if (this.tcpChannel != null)
                    return (IPEndPoint)tcpChannel.LocalAddress;
                return null;
            }
        }

        protected bool InitTcpClient(ulong connId, IPEndPoint ep)
        {
            if(ep != null)
                Log.Info(string.Format("init_tcp_client {0} {1}", connId, ep.ToString()));
            if (ep == null)
            {
                var addr = Global.IdManager.GetHostAddr(connId);//, false);
                if (addr == null) 
                    return false; 

                var parts = addr.Split(':');
                return InitTcpClient(new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            }
            return InitTcpClient(ep);
        }

        protected bool InitKcpClient(ulong connId, IPEndPoint ep)
        {
            if (ep != null)
                Log.Info(string.Format("init_kcp_client {0} {1}", connId, ep.ToString()));
            if (ep == null)
            {
                var addr = Global.IdManager.GetHostAddr(connId);//, false);
                if (addr == null) 
                    return false;

                var parts = addr.Split(':');
                return InitKcpClient(new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            } 
            return InitKcpClient(ep);
        }

        protected bool InitTcpClient(IPEndPoint ep)
        {
            tcpClient = TcpHostClient.Create(ep); 
            if (tcpClient == null)
                return false;

            tcpClient.OnReceive += (ch, buffer) =>
            {
                OnReceive?.Invoke(this, buffer);
            };

            tcpClient.OnClose += (ch) => 
            { 
                OnClose?.Invoke(this); 
            };

            tcpClient.OnException += (ch, ex) => 
            { 
                OnException?.Invoke(this, ex); 
            };

            Log.Info(string.Format("init_tcp_client_localaddr@{0}", tcpClient.LocalAddress));
            return true;
        }
 
        protected bool InitKcpClient(IPEndPoint ep)
        { 
            kcpClient = KcpHostClient.Create(ep);
            kcpClient.OnReceive += (kcp, buffer)=> {
                OnReceive?.Invoke(this, buffer); 
            };
            kcpClient.OnClose += (kcp) => { 
                OnClose?.Invoke(this); 
            };
            kcpClient.OnException += (ch, ex) => { 
                OnException?.Invoke(this, ex); 
            };
            Log.Info(string.Format("init_kcp_client_localaddr@{0}", kcpClient.LocalAddress));
            return true;
        }

        public static NetPeer Create(ulong connId, Ukcp kcpCh)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.kcpChannel = kcpCh;
            obj.netType = NetworkType.KCP;
            return obj;
        }

        public static NetPeer Create(ulong connId, IChannel tcpCh)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.tcpChannel = tcpCh;
            obj.netType = NetworkType.TCP;
            return obj;
        }

        public static NetPeer Create(ulong connId, IPEndPoint addr, NetworkType netType)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.netType = netType;
            if (netType == NetworkType.TCP)
            {
                if (!obj.InitTcpClient(connId, addr))
                    return null;
            }
            else
            {
                if (!obj.InitKcpClient(connId, addr))
                    return null;
            }
            return obj;
        }

        public static NetPeer Create(IPEndPoint ep, NetworkType netType)
        { 
            var obj = new NetPeer();
            obj.ConnId = Basic.GenID64FromName(ep.ToString());
            obj.netType = netType;
            if (netType == NetworkType.TCP)
            {
                if (!obj.InitTcpClient(ep))
                    return null;
            }
            else
            {
                if (!obj.InitKcpClient(ep))
                    return null;
            }
            return obj;
        }
 
        ~NetPeer()
        {
             
        }

        static byte[] pingBytes = new byte[] { (byte)OpCode.PING };
        static byte[] pongBytes = new byte[] { (byte)OpCode.PONG };

        public void Send(byte[] bytes)
        {
            this.Send(Unpooled.WrappedBuffer(bytes));
        }

        public void Send(IByteBuffer buffer)
        {
            if (kcpChannel != null) 
            { kcpChannel.write(buffer); return; }
            if (kcpClient != null) 
            { kcpClient.Send(buffer); return; }
            if (tcpChannel != null) 
            { tcpChannel.WriteAndFlushAsync(buffer); return; }
            if (tcpClient != null) 
            { tcpClient.Send(buffer); return; }
        }

        //public async Task SendAsync(byte[] bytes)
        //{
        //    await Task.Run(() => {
        //        this.Send(bytes);
        //    });
        //}

        //public void Send(Packet packet)
        //{ 
        //    this.Send(packet.Pack());
        //}

        public void Stop()
        {
            if (!IsAlive)
                return;

            IsAlive = false;

            //if(Global.Host.IsClientMode || this.netType == NetworkType.KCP)
            if(this.netType == NetworkType.KCP)
                this.GoodBye();

            kcpClient?.Stop();
            tcpClient?.Stop();
            tcpChannel?.CloseAsync();
            kcpChannel?.close();

            kcpClient = null;
            tcpClient = null;
            tcpChannel = null;
            kcpChannel = null;
        }

        public void Ping()
        {
            lastTickTime = Fenix.Common.Utils.TimeUtil.GetTimeStampMS2();
            this.Send(pingBytes);
        }

        public void Pong()
        {
            this.Send(pongBytes);
            this.lastTickTime = Fenix.Common.Utils.TimeUtil.GetTimeStampMS2();
        }

        public void GoodBye()
        {
            this.Send(new byte[] { (byte)OpCode.GOODBYE });
        }

        public void Register()
        {
            //var buffer = Unpooled.DirectBuffer();
            //buffer.WriteIntLE((int)OpCode.REGISTER_REQ);
            //buffer.WriteLongLE((long)Global.Host.Id);
            //buffer.WriteBytes(Encoding.UTF8.GetBytes(Global.Host.UniqueName));

            using (var m = new MemoryStream())
            {
                using (var writer = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Little, m))
                {
                    writer.Write(OpCode.REGISTER_REQ);
                    writer.Write(Global.Host.Id);
                    writer.Write(Encoding.UTF8.GetBytes(Global.Host.UniqueName));
                    this.Send(m.ToArray());
                }
            }

            //this.Send(buffer);
            //buffer.Release();
        }
    }
}
