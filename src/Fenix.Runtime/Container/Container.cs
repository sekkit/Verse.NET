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
using Basic = Fenix.Common.Utils.Basic;
using static Fenix.Common.RpcUtil;
using System.Collections.Concurrent;
using Fenix.Common.Utils;
using System.Reflection;
using Fenix.Common.Attributes;

namespace Fenix
{
    //一个内网IP，必须
    //一个外网IP
    
    public class Container : RpcModule
    {
        public static Container Instance = null;

        //public Container(uint instanceId, string tag, IPEndPoint localAddress, KcpContainerServer kcpServer, TcpContainerServer tcpServer)
        //{
        //    Id = instanceId;
        //    Tag = tag;
        //    LocalAddress = localAddress;
        //    this.kcpServer = kcpServer;
        //    this.tcpServer = tcpServer; 
        //}  
        

        public uint Id { get; set; }

        public string Tag { get; set; }

        public string UniqueName { get; set; }
        
        protected IPEndPoint LocalAddress { get; set; }

        protected KcpContainerServer kcpServer { get; set; }
        
        protected TcpContainerServer tcpServer { get; set; }

        protected ConcurrentDictionary<UInt32, Actor> actorDic = new ConcurrentDictionary<UInt32, Actor>();
         
        protected Container(string name, int port=0): base()
        {
            if (name == null)
                this.UniqueName = Basic.GenID64().ToString();
            else
                this.UniqueName = name;
            this.Id = Basic.GenID32FromName(this.UniqueName);

            string addr = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            IPAddress ipAddr = IPAddress.Parse(addr);

            if (port == 0)
                port = Basic.GetAvailablePort(ipAddr);
            
            this.LocalAddress = new IPEndPoint(ipAddr, port);
            
            this.RegisterGlobalManager(this);

            this.SetupKcpServer();

            this.InitTcp();
        }

        protected async Task InitTcp()
        {
            await this.SetupTcpServer();

            //await this.SetupTcpClient();
        }

        public static Container Create(string name, int port)
        {
            if (Instance != null)
                return Instance;
            var c = new Container(name, port);
            Instance = c;
            return Instance;
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

        private void KcpClient_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            string data = StringUtil.ToHexString(buffer.ToArray()); 
            Console.WriteLine("FROM_SERVER:" + data);
            ukcp.writeMessage(buffer);
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
            tcpServer = await TcpContainerServer.Create(this.LocalAddress);
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
            if (buffer.ReadableBytes == 1)
            {
                byte protoCode = buffer.ReadByte();
                if (protoCode == (byte)ProtoCode.PING)
                {
                    peer.Send(new byte[] { (byte)ProtoCode.PONG });
                }
                else if(protoCode == (byte)ProtoCode.GOODBYE)
                {
                    //删除这个连接
                    NetManager.Instance.DeregisterChannel(channel);
                    peer.Send(new byte[] { (byte)ProtoCode.GOODBYE });
                }
                
                return;
            }
            else
            {
                uint protoCode = buffer.ReadUnsignedIntLE();
                if (protoCode >= (uint)ProtoCode.CALL_ACTOR_METHOD)
                {
                    ulong msgId = (ulong)buffer.ReadLongLE(); 
                    uint fromActorId = buffer.ReadUnsignedIntLE();
                    uint toActorId = buffer.ReadUnsignedIntLE(); 
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);

                    //var msg = MessagePackSerializer.Deserialize<ActorMessage>(bytes);
                    var msg = Packet.Create(msgId, protoCode, fromActorId, toActorId, bytes);
                    HandleIncomingActorMessage(peer, msg);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var packet = Packet.Create(msgId, protoCode, bytes);

                    this.CallMethod(peer.ConnId, this.Id, packet);
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

        protected void RegisterGlobalManager(Container container)
        {
            Global.IdManager.RegisterContainer(container, this.LocalAddress.ToString());
        }

        protected void RegisterGlobalManager(Actor actor)
        {
            Global.IdManager.RegisterActor(actor, this); 
        }

        public Actor GetActor(uint actorId)
        {
            Actor a;
            this.actorDic.TryGetValue(actorId, out a);
            return a;
        }

        protected void HandleIncomingActorMessage(NetPeer fromPeer, Packet packet)
        {
            var remoteContainerId = fromPeer.ConnId;
            CallActorMethod(remoteContainerId, packet);
        }

        [ServerOnly]
        protected void CreateActor(string typename, string name, Action<ErrCode> callback)
        {
            var type = Global.TypeManager.Get(typename);
            var newActor = Actor.Create(type, name);
            this.RegisterGlobalManager(newActor);

            actorDic[newActor.Id] = newActor;

            callback(ErrCode.OK);
        }

        public Actor CreateActor<T>(string name) where T : Actor
        { 
            var newActor = Actor.Create(typeof(T), name);
            this.RegisterGlobalManager(newActor);
            actorDic[newActor.Id] = newActor;
            return newActor;
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
        protected void CallActorMethod(uint fromContainerId, Packet packet) //uint actorId, uint methodId, object[] args)
        {
            var actor = this.actorDic[packet.ToActorId]; 
            actor.CallMethod(fromContainerId, this.Id, packet);
        }

        public virtual void Update()
        {
            foreach(var a in this.actorDic.Keys)
            {
                this.actorDic[a].Update();
            }
        } 
    }
}
