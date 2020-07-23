
using DotNetty.Buffers; 
using DotNetty.KCP;
using DotNetty.Transport.Channels;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Fenix
{
    public class NetPeer
    { 
        public uint ConnId { get; set; }

        protected Ukcp kcpChannel { get; set; }
        
        protected IChannel tcpChannel { get; set; }

        protected TcpHostClient tcpClient { get; set; }

        protected KcpHostClient kcpClient { get; set; }

        public event Action<NetPeer, IByteBuffer> OnReceive;

        public event Action<NetPeer> OnClose;

        public event Action<NetPeer, Exception> OnException;

        public event Action<NetPeer, IByteBuffer> OnSend;

        public NetworkType networkType;

        public bool IsActive
        {
            get
            {
                if (this.tcpChannel != null)
                    return tcpChannel.Active;
                if (this.kcpChannel != null)
                    return kcpChannel.isActive();
                if (this.kcpClient != null)
                    return true;
                if (this.tcpClient != null)
                    return this.tcpClient.IsActive;
                return false;
            }
        }

        // public static NetPeer Create(IChannel channel)
        // {
        //     var obj = new NetPeer(); 
        //     obj.ConnId = 0;
        //     obj.tcpChannel = channel;
        //     return obj;
        // }
        
        protected NetPeer()
        {

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

        protected bool InitTcpClient(uint connId, IPEndPoint ep)
        {
            if (ep == null)
            {
                var addr = Global.IdManager.GetHostAddr(connId);
                if (addr == null)
                {
                    //host 不存在
                    return false;
                }
                var parts = addr.Split(':');
                return InitTcpClient(new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            }
            return InitTcpClient(ep);
        }

        protected bool InitKcpClient(uint connId, IPEndPoint ep)
        {
            if(ep == null)
            {
                var addr = Global.IdManager.GetHostAddr(connId);
                if (addr == null)
                {
                    //host 不存在
                    return false;
                }

                var parts = addr.Split(':');
                return InitKcpClient(new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            } 
            return InitKcpClient(ep);
        }

        protected bool InitTcpClient(IPEndPoint ep)
        {  
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);

            tcpClient = TcpHostClient.Create(ep);

            if (tcpClient == null)
                return false;

            tcpClient.Receive += TcpClient_OnReceive;
            tcpClient.Close += (ch) => { OnClose?.Invoke(this); };
            tcpClient.Exception += (ch, ex) => { OnException?.Invoke(this, ex); };
            return true;
        }

        protected bool InitKcpClient(IPEndPoint ep)
        {
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);

            kcpClient = KcpHostClient.Create(ep);
            kcpClient.OnReceive += (kcp, buffer)=> { OnReceive?.Invoke(this, buffer); };
            kcpClient.OnClose += (kcp) => { OnClose?.Invoke(this); };
            kcpClient.OnException += (ch, ex) => { OnException?.Invoke(this, ex); };
            return true;
        }

        public static NetPeer Create(uint connId, Ukcp kcpCh)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.kcpChannel = kcpCh;
            obj.networkType = NetworkType.KCP;
            return obj;
        }

        public static NetPeer Create(uint connId, IChannel tcpCh)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.tcpChannel = tcpCh;
            obj.networkType = NetworkType.TCP;
            return obj;
        }

        public static NetPeer Create(uint connId, IPEndPoint addr, NetworkType netType)
        {
            var obj = new NetPeer();
            obj.ConnId = connId;
            obj.networkType = netType;
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

        public static NetPeer Create(string ip, int port, NetworkType netType)
        {
            var obj = new NetPeer();
            obj.ConnId = 0;
            obj.networkType = netType;
            if (netType == NetworkType.TCP)
            {
                if (!obj.InitTcpClient(new IPEndPoint(IPAddress.Parse(ip), port)))
                    return null;
            }
            else
            {
                if (!obj.InitKcpClient(new IPEndPoint(IPAddress.Parse(ip), port)))
                    return null;
            } 
            return obj;
        }

        private void TcpClient_OnReceive(IChannel channel, IByteBuffer buffer)
        {
            OnReceive?.Invoke(this, buffer);
        }

        private void KcpClient_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            OnReceive?.Invoke(this, buffer);
        }

        ~NetPeer()
        {
            if (tcpClient != null)
                tcpClient.Receive -= TcpClient_OnReceive;
        }

        public void Send(byte[] bytes)
        {
            kcpChannel?.send(bytes);
            tcpChannel?.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
            kcpClient?.Send(bytes);
            tcpClient?.Send(bytes);
        }

        public async Task SendAsync(byte[] bytes)
        {
            await Task.Run(() => {
                this.Send(bytes);
            });
        }

        public void Send(Packet packet)
        { 
            this.Send(packet.Pack());
        }

        public void Stop()
        {
            kcpClient?.Stop();
            tcpClient?.Stop();
            tcpChannel?.CloseAsync();
            kcpChannel?.notifyCloseEvent();
        }
    }
}
