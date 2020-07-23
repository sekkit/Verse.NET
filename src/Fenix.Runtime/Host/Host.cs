//Fenix, Inc.
//

using System;
using System.Net;
using System.Net.NetworkInformation;
using DotNetty.KCP;
using DotNetty.Buffers; 
using DotNetty.Common.Utilities; 
using DotNetty.Transport.Channels;
using Fenix.Common;
using Basic = Fenix.Common.Utils.Basic; 
using System.Collections.Concurrent;
using Fenix.Common.Utils; 
using Fenix.Common.Attributes;
using System.Threading;

namespace Fenix
{
    //一个内网IP，必须
    //一个外网IP
    
    public class Host : RpcModule
    {
        public static Host Instance = null; 

        public uint Id { get; set; } // 实例ID，全局唯一

        public string Tag { get; set; }

        public string UniqueName { get; set; }
        
        protected IPEndPoint LocalAddress { get; set; }

        protected KcpHostServer kcpServer { get; set; }
        
        protected TcpHostServer tcpServer { get; set; }

        protected NetPeer clientPeer { get; set; } 

        protected bool isClientMode { get; set; }

        protected ConcurrentDictionary<UInt32, Actor> actorDic = new ConcurrentDictionary<UInt32, Actor>();
         
        protected Host(string name, string ip, int port=0, bool clientMode=false) : base()
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

            this.isClientMode = clientMode;

            this.RegisterGlobalManager(this);

            if (!clientMode)
            {
                this.SetupKcpServer();
                this.SetupTcpServer();
            }
            else
            {
                clientPeer = NetPeer.Create(ip, port);
                clientPeer.OnReceive += Server_OnReceive;
                clientPeer.OnClose += Server_OnClose;
                clientPeer.OnException += Server_OnException;

                this.Id = Basic.GenID32FromName(clientPeer.LocalAddress.ToString());

                Thread thread = new Thread(new ThreadStart(Heartbeat));//创建线程 
                thread.Start();
            }

            Log.Info(string.Format("{0} is running at {1}", this.UniqueName, LocalAddress.ToString()));
        }
 
        public static Host Create(string name, string ip, int port, bool clientMode)
        { 
            if (Instance != null)
                return Instance;

            var c = new Host(name, ip, port, clientMode);
            Instance = c;
            return Instance;
        } 
        
        #region KCP

        protected void SetupKcpServer()
        {
            kcpServer = KcpHostServer.Create(this.LocalAddress);
            kcpServer.OnConnect += KcpServer_OnConnect;
            kcpServer.OnReceive += KcpServer_OnReceive;
            kcpServer.OnClose += KcpServer_OnClose;
            kcpServer.OnException += KcpServer_OnException;
        }
 
        //private long last_ts = DateTime.Now.Ticks;
        protected KcpHostClient CreateKcpClient(IPEndPoint remoteAddreses)
        {
            var kcpClient = KcpHostClient.Create(remoteAddreses); 
            kcpClient.OnReceive += KcpClient_OnReceive;
            kcpClient.OnClose += KcpClient_OnClose;
            kcpClient.OnException += KcpClient_OnException;
            return kcpClient;
        } 

        protected void KcpServer_OnConnect(Ukcp ukcp)
        {
            //新连接
            NetManager.Instance.RegisterKcp(ukcp);
            //ulong hostId = Global.IdManager.GetHostId(channel.RemoteAddress.ToString());
            Console.WriteLine(string.Format("kcp_client_connected {0}", ukcp.user().RemoteAddress.ToString()));
        }

        private void KcpServer_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            var peer = NetManager.Instance.GetPeer(ukcp);
            //如果包是从客户端发过来的
             
            //如果是从服务端过来的

            //var peer = NetManager.Instance.GetPeer(ukcp.);

            //Ping/Pong msg process 
            if (buffer.ReadableBytes == 1)
            {
                byte protoCode = buffer.ReadByte();
                if (protoCode == (byte)ProtoCode.PING)
                {
                    peer.Send(new byte[] { (byte)ProtoCode.PONG });
                }
                else if (protoCode == (byte)ProtoCode.GOODBYE)
                {
                    //删除这个连接
                    NetManager.Instance.DeregisterKcp(ukcp);
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
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, fromActorId, toActorId, bytes);
                    HandleIncomingActorMessage(peer, packet);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, 0, 0, bytes);

                    this.CallMethod(peer.ConnId, this.Id, packet);
                }
            }

            ///*
            //short curCount = buffer.GetShort(buffer.ReaderIndex);
            //Console.WriteLine(Thread.CurrentThread.Name + " 收到消息 " + curCount); 

            //if (curCount == -1)
            //{
            //    ukcp.notifyCloseEvent();
            //}
            //*/
            //var bytes = new byte[buffer.ReadableBytes];
            //buffer.ReadBytes(bytes);
            //string data = StringUtil.ToHexString(bytes);
            ////string data2 = buffer.GetString(0, buffer.ReadableBytes, Encoding.UTF8);
            
            ////count++; 
            ////var cur_ts = DateTime.Now.Ticks;
            //Console.WriteLine("FROM_CLIENT:" + data); //stopWatch.Elapsed.TotalMilliseconds.ToString());
            ////last_ts = cur_ts;  
            //ukcp.writeMessage(Unpooled.WrappedBuffer(bytes));
        }
        
        private void KcpServer_OnException(Ukcp ukcp, Exception ex)
        {
            Log.Error(ex.StackTrace);
            NetManager.Instance.DeregisterKcp(ukcp);
        }

        private void KcpServer_OnClose(Ukcp ukcp)
        {
            NetManager.Instance.DeregisterKcp(ukcp);
        }

        private void KcpClient_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            string data = StringUtil.ToHexString(buffer.ToArray()); 
            Console.WriteLine("FROM_SERVER:" + data);
            //ukcp.writeMessage(buffer);
        }
        
        private void KcpClient_OnException(Ukcp arg2, Exception ex)
        {
            Log.Error(ex.StackTrace);
            NetManager.Instance.DeregisterKcp(arg2);
        }

        private void KcpClient_OnClose(Ukcp obj)
        {
            NetManager.Instance.DeregisterKcp(obj);
        }

        #endregion

        #region TCP
        protected TcpHostServer SetupTcpServer()
        { 
            tcpServer = TcpHostServer.Create(this.LocalAddress);
            tcpServer.Connect   += OnTcpIncomingConnect;
            tcpServer.Receive   += OnTcpServerReceive;
            tcpServer.Close     += OnTcpServerClose;
            tcpServer.Exception += OnTcpServerException;
            return tcpServer;
        }

        protected TcpHostClient CreateTcpClient(IPEndPoint remoteAddress)
        {
            var tcpClient = TcpHostClient.Create(remoteAddress);
            tcpClient.Receive    += OnTcpClientReceive;
            tcpClient.Close      += OnTcpClientClose;
            tcpClient.Exception  += OnTcpClientException;
            return tcpClient;
        }
        
        void OnTcpIncomingConnect(IChannel channel)
        {
            //新连接
            NetManager.Instance.RegisterChannel(channel);
            ulong hostId = Global.IdManager.GetHostId(channel.RemoteAddress.ToString());
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
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, fromActorId, toActorId, bytes);
                    HandleIncomingActorMessage(peer, packet);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, 0, 0, bytes);

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
            NetManager.Instance.DeregisterChannel(channel);
        }
        
        void OnTcpServerException(IChannel channel, Exception ex)
        {
            Log.Error(ex.StackTrace);
            NetManager.Instance.DeregisterChannel(channel); 
        }
        
        void OnTcpClientReceive(IChannel channel, IByteBuffer buffer)
        {
            Console.Write("clientRecv");
        }
        
        void OnTcpClientClose(IChannel channel)
        {
            NetManager.Instance.DeregisterChannel(channel);
        }
        
        void OnTcpClientException(IChannel channel, Exception ex)
        {
            Log.Error(ex.StackTrace);
            NetManager.Instance.DeregisterChannel(channel);
        }

        #endregion

        protected void Heartbeat()
        {
            while (true)
            {
                Ping();
                Thread.Sleep(5000);
            }
        }

        protected void Ping()
        {
            clientPeer.Send(new byte[] { (byte)ProtoCode.PING });
        }

        private void Server_OnException(NetPeer peer, Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            clientPeer.Stop();
            clientPeer = null;
        }

        private void Server_OnClose(NetPeer obj)
        {
            clientPeer.Stop();
            clientPeer = null;
        }

        private void Server_OnReceive(NetPeer peer, IByteBuffer buffer)
        {
            //Ping/Pong msg process 
            if (buffer.ReadableBytes == 1)
            {
                byte protoCode = buffer.ReadByte();
                if (protoCode == (byte)ProtoCode.PING)
                {
                    peer.Send(new byte[] { (byte)ProtoCode.PONG });
                }
                else if (protoCode == (byte)ProtoCode.PONG)
                {
                    //心跳包
                    //TODO 超时不心跳断开

                }
                else if (protoCode == (byte)ProtoCode.GOODBYE)
                {
                    //删除这个连接
                    NetManager.Instance.Deregister(peer);
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
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, fromActorId, toActorId, bytes);

                    HandleIncomingActorMessage(peer, packet);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, 0, 0, bytes);

                    this.CallMethod(peer.ConnId, this.Id, packet);
                }
            }

            Console.WriteLine(StringUtil.ToHexString(buffer.ToArray()));
        }


        protected void RegisterGlobalManager(Host host)
        {
            Global.IdManager.RegisterHost(host, this.LocalAddress.ToString());
        }

        protected void RegisterGlobalManager(Actor actor)
        {
            Global.IdManager.RegisterActor(actor, this);
            Global.TypeManager.RegisterActorType(actor);
        }

        public Actor GetActor(uint actorId)
        { 
            if (this.actorDic.TryGetValue(actorId, out Actor a))
                return a;
            return null;
        }

        protected void HandleIncomingActorMessage(NetPeer fromPeer, Packet packet)
        {
            var remoteHostId = fromPeer.ConnId;
            CallActorMethod(remoteHostId, packet);
        }

        [ServerOnly]
        public void CreateActor(string typename, string name, Action<DefaultErrCode> callback)
        {
            var a = CreateActor(typename, name);
            if (a != null)
                callback(DefaultErrCode.OK);
            else
                callback(DefaultErrCode.ERROR);
        }

        public Actor CreateActor<T>(string name) where T : Actor
        { 
            var newActor = Actor.Create(typeof(T), name);
            this.RegisterGlobalManager(newActor);
            actorDic[newActor.Id] = newActor;
            return newActor;
        }
        
        public Actor CreateActor(string typename, string name) 
        {
            var type = Global.TypeManager.Get(typename);
            var newActor = Actor.Create(type, name);
            this.RegisterGlobalManager(newActor);

            actorDic[newActor.Id] = newActor;
            return newActor;
        }

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
        protected void CallActorMethod(uint fromHostId, Packet packet)  
        {
            var actor = this.actorDic[packet.ToActorId]; 
            actor.CallMethod(fromHostId, this.Id, packet);
        }

        public virtual void Update()
        {
            foreach(var a in this.actorDic.Keys)
            {
                this.actorDic[a].Update();
            }

            //Log.Info(string.Format("C: {0}", rpcDic.Count));
        }

        public dynamic GetService(string name)
        {
            return Global.GetActorRef(name, null);
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(name, null);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(uid, null);
        }

        public ActorRef GetActorRef(string name)
        {
            return Global.GetActorRef(name, null);
        }

        public T GetActorRef<T>(string name) where T: ActorRef
        {
            return (T)Global.GetActorRef(name, null);
        }
    }
}
