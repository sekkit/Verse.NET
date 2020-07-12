//Fenix, Inc.
//

using System;
using System.Net;
using System.Net.NetworkInformation;
using DotNetty.KCP;
using DotNetty.Buffers; 
using DotNetty.Common.Utilities;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Fenix.Common;
using Fenix.Common.Utils;
using MessagePack;
using MessagePack.Formatters;
using NetUtil = Fenix.Common.Utils.NetUtil;
using static Fenix.Common.RpcUtil;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Reflection;
using System.Linq;

namespace Fenix
{
    //一个内网IP，必须
    //一个外网IP
    
    public class Container : RpcModule
    {
        public Container(uint instanceId, string tag, IPEndPoint localAddress, KcpContainerServer kcpServer, TcpContainerServer tcpServer)
        {
            Id = instanceId;
            Tag = tag;
            LocalAddress = localAddress;
            this.kcpServer = kcpServer;
            this.tcpServer = tcpServer;
        } 

        public uint Id { get; set; }

        public string Tag { get; set; }

        public string UniqueName { get; set; }
        
        protected IPEndPoint LocalAddress { get; set; }

        protected KcpContainerServer kcpServer { get; set; }
        
        protected TcpContainerServer tcpServer { get; set; }

        protected ConcurrentDictionary<UInt32, Actor> actorDic = new ConcurrentDictionary<UInt32, Actor>();
         
        protected Container(int port=0): base()
        {  
            string addr = NetUtil.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            IPAddress ipAddr = IPAddress.Parse(addr);

            if (port == 0)
                port = NetUtil.GetAvailablePort(ipAddr);
            
            this.LocalAddress = new IPEndPoint(ipAddr, port);
            
            this.RegisterToIdManager(this);

            this.SetupKcpServer();

            this.InitTcp();
        }

        protected async Task InitTcp()
        {
            await this.SetupTcpServer();

            //await this.SetupTcpClient();
        }

        public static Container Create(int port)
        {
            return new Container(port);
        }
        
        #region KCP

        protected void SetupKcpServer()
        {
            kcpServer = KcpContainerServer.Create(this.LocalAddress);
            kcpServer.OnReceive += KcpServer_OnReceive;
            kcpServer.OnClose += KcpServer_OnClose;
            kcpServer.OnException += KcpServer_OnException;
        }
 
        //private long last_ts = DateTime.Now.Ticks;
        protected KcpContainerClient CreateKcpClient(IPEndPoint remoteAddreses)
        {
            var kcpClient = KcpContainerClient.Create(remoteAddreses);
            kcpClient.OnReceive += KcpClient_OnReceive;
            kcpClient.OnClose += KcpClient_OnClose;
            kcpClient.OnException += KcpClient_OnException;
            return kcpClient;
        }

        private void KcpServer_OnReceive(byte[] bytes, Ukcp ukcp)
        {
            /*
            short curCount = buffer.GetShort(buffer.ReaderIndex);
            Console.WriteLine(Thread.CurrentThread.Name + " 收到消息 " + curCount); 

            if (curCount == -1)
            {
                ukcp.notifyCloseEvent();
            }

            var bytes = new byte[buffer.ReadableBytes];
            buffer.GetBytes(0, bytes);*/
            string data = StringUtil.ToHexString(bytes);
            //string data2 = buffer.GetString(0, buffer.ReadableBytes, Encoding.UTF8);
            
            //count++; 
            //var cur_ts = DateTime.Now.Ticks;
            //Console.WriteLine("FROM_CLIENT:" + data + " => " + count.ToString() + ":" + ((cur_ts - last_ts)/10000.0).ToString()); //stopWatch.Elapsed.TotalMilliseconds.ToString());
            //last_ts = cur_ts;  
            ukcp.writeMessage(Unpooled.WrappedBuffer(bytes));
        }
        
        private void KcpServer_OnException(Exception ex, Ukcp ukcp)
        {
            
        }

        private void KcpServer_OnClose(Ukcp ukcp)
        {
            
        }

        private void KcpClient_OnReceive(byte[] bytes, Ukcp ukcp)
        {
            string data = StringUtil.ToHexString(bytes); 
            Console.WriteLine("FROM_SERVER:" + data + " => " + Encoding.UTF8.GetString(bytes));
            ukcp.writeMessage(Unpooled.WrappedBuffer(bytes));
        }
        
        private void KcpClient_OnException(Exception ex, Ukcp arg2)
        {
            
        }

        private void KcpClient_OnClose(Ukcp obj)
        {
            
        }

        #endregion

        #region TCP
        protected async Task<TcpContainerServer> SetupTcpServer()
        { 
            tcpServer = await TcpContainerServer.Create(this.LocalAddress);//this.ip, this.port);
            tcpServer.Connect   += OnTcpIncomingConnect;
            tcpServer.Receive   += OnTcpServerReceive;
            tcpServer.Close     += OnTcpServerClose;
            tcpServer.Exception += OnTcpServerException;
            return tcpServer;
        }

        protected async Task<TcpContainerClient> CreateTcpClient(IPEndPoint remoteAddress)
        {
            var tcpClient = await TcpContainerClient.Create(remoteAddress);
            tcpClient.Receive    += OnTcpClientReceive;
            tcpClient.Close      += OnTcpClientClose;
            tcpClient.Exception  += OnTcpClientException;
            return tcpClient;
        }
        
        
        void OnTcpIncomingConnect(IChannel channel)
        {
            //新连接
            NetManager.Instance.RegisterChannel(channel);
            ulong containerId = Global.IdManager.GetContainerId(channel.RemoteAddress.ToString());
            Console.WriteLine(channel.RemoteAddress.ToString());
        }
        
        void OnTcpServerReceive(IChannel channel, IByteBuffer buffer)
        {
            var peer = NetManager.Instance.GetPeer(channel);

            //Ping/Pong msg process
            if(buffer.ReadableBytes == 1)
            {
                byte data = buffer.ReadByte();
                if(data == (byte)DefaultProtocol.PING)
                {
                    peer.Send(new byte[] { (byte)DefaultProtocol.PONG });
                }
                else if(data == (byte)DefaultProtocol.GOODBYE)
                {
                    //删除这个连接
                    NetManager.Instance.DeregisterChannel(channel);
                    peer.Send(new byte[] { (byte)DefaultProtocol.GOODBYE });
                }
                
                return;
            }
            else
            {
                byte pid = buffer.ReadByte();
                if (pid == (byte)DefaultProtocol.CALL_ACTOR_METHOD)
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    uint protocolId = buffer.ReadUnsignedIntLE();
                    uint fromActorId = buffer.ReadUnsignedIntLE();
                    uint toActorId = buffer.ReadUnsignedIntLE(); 
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);

                    //var msg = MessagePackSerializer.Deserialize<ActorMessage>(bytes);
                    var msg = Message.Create(msgId, protocolId, fromActorId, toActorId, bytes);
                    HandleIncomingActorMessage(peer, msg);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var msg = Message.Create(msgId, pid, bytes);

                    this.CallMethod(peer.ConnId, msg);
                }
            }
            
            //解析包
            //var msg = MessagePackSerializer.Deserialize<Message>(bytes);
            //Console.WriteLine(MessagePackSerializer.SerializeToJson(msg)); 
            //
        }
        
        void OnTcpServerClose(IChannel channel)
        {
            
        }
        
        void OnTcpServerException(IChannel channel)
        {
            
        }
        
        void OnTcpClientReceive(IChannel channel, IByteBuffer buffer)
        {
            Console.Write("clientRecv");
        }
        
        void OnTcpClientClose(IChannel channel)
        {
            
        }
        
        void OnTcpClientException(IChannel channel)
        {
            
        }
        
        #endregion

        protected void RegisterToIdManager(Container container)
        {
            Global.IdManager.RegisterContainer(container.Id, string.Format("{0}", this.LocalAddress.ToString()));
        }

        protected void RegisterToIdManager(Actor actor)
        {
            Global.IdManager.RegisterActor(actor.Id, this.Id);
        }

        public void HandleIncomingActorMessage(NetPeer fromPeer, Message msg)
        { 
            //Global.GetActor("FightService").rpc_spawn_actor(); 
            //Global.GetActor("FightService").SpawnActor<MatchService>("Hello");
            //Global.GetActor<FightService>().SpawnActor<MatchService>("Hello");
        }
        
        [ServerOnly]
        protected void SpawnActor(string typename, string name, Action<ErrCode> callback)
        {
            var type = Global.TypeManager.Get(typename);
            var newActor = Actor.Create(type);
            this.RegisterToIdManager(newActor);

            actorDic[newActor.Id] = newActor;

            callback(ErrCode.OK);
        }

        //public void __SpawnActor(RpcCommand cmd)
        //{
        //    var msg = cmd.ToMessage<SpawnActorMsg>();
        //    this.SpawnActor(msg.typeName, msg.name, (code) =>
        //    {

        //        cmd.Callback(cb_msg);
        //    });
        //}

        //迁移actor
        [ServerOnly]
        protected void MigrateActor(uint actorId)
        {
            
        }

        [ServerOnly]
        //移除actor
        protected void RemoveActor(uint actorId)
        {
            
        }
  
        //调用Actor身上的方法
        protected void CallActorMethod(uint fromPeerId, Message msg) //uint actorId, uint methodId, object[] args)
        {
            var actor = this.actorDic[msg.toActorId];
            actor.CallMethod(fromPeerId, msg);
        }

        public virtual void Update()
        {
            
        } 
    }
}
