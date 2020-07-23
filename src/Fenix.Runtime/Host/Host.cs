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
using System.Reflection.PortableExecutable;

namespace Fenix
{
    //一个内网IP，必须
    //一个外网IP
    
    public partial class Host : RpcModule
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

        //public IPEndPoint ExternalAddress { get; set; }

        //public IPEndPoint InternalAddress { get; set; }

        //protected ConcurrentDictionary<>

        protected ConcurrentDictionary<UInt32, Actor> actorDic = new ConcurrentDictionary<UInt32, Actor>();
         
        protected Host(string name, string ip, int port=0, bool clientMode=false) : base()
        {
            this.isClientMode = clientMode;

            //如果是客户端，则用本地连接做为id
            //如果是服务端，则从名称计算一个id, 方便路由查找
            if (!clientMode)
            {
                string _ip = ip;
                int _port = port;

                if (ip == "auto") 
                    _ip = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);  

                if (port == 0)
                    _port = Basic.GetAvailablePort(IPAddress.Parse(_ip));

                this.LocalAddress = new IPEndPoint(IPAddress.Parse(_ip), _port);

                string addr = LocalAddress.ToString();

                if (name == null)
                    this.UniqueName = Basic.GenID64().ToString() ;
                else
                    this.UniqueName = name;

                this.Id = Basic.GenID32FromName(this.UniqueName); 

                this.RegisterGlobalManager(this);
            
                this.SetupKcpServer();
                this.SetupTcpServer();
            }
            else
            {
                clientPeer = NetManager.Instance.CreatePeer(ip, port, NetworkType.KCP); 
                if(clientPeer != null)
                {
                    clientPeer.OnReceive += Server_OnReceive;
                    clientPeer.OnClose += Server_OnClose;
                    clientPeer.OnException += Server_OnException;

                    if (name == null)
                        this.UniqueName = Basic.GenID64().ToString();
                    else
                        this.UniqueName = name;

                    this.Id = Basic.GenID32FromName(clientPeer.LocalAddress.ToString());

                    this.LocalAddress = clientPeer.LocalAddress;

                    var thread = new Thread(new ThreadStart(Heartbeat));//创建线程 
                    thread.Start();
                }
                else
                {
                    throw new Exception("unable_to_connect_server"); 
                }
            }

            if (!this.isClientMode)
            {
                Log.Info(string.Format("{0} is running at {1} as ServerMode", this.UniqueName, LocalAddress.ToString()));
            }
            else
            {
                Log.Info(string.Format("{0} is running at {1} as ClientMode", this.UniqueName, LocalAddress.ToString()));
            }
        }
 
        public static Host Create(string name, string ip, int port, bool clientMode)
        {  
            if (Instance != null)
                return Instance;
            try
            {
                var c = new Host(name, ip, port, clientMode);
                Instance = c;
                return Instance;
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace); 
            }
            return null;
        } 

        public static Host CreateClient(string ip, int port)
        {
            return Create(Basic.GenID32FromName(string.Format("{0}:{1}", ip, port)).ToString(), ip, port, true); 
        }

        public static Host CreateServer(string name, string ip, int port)
        {
            return Create(name, ip, port, false);
        }
        
        #region KCP

        protected KcpHostServer SetupKcpServer()
        {
            kcpServer = KcpHostServer.Create(this.LocalAddress);
            kcpServer.OnConnect += KcpServer_OnConnect;
            kcpServer.OnReceive += KcpServer_OnReceive;
            kcpServer.OnClose += KcpServer_OnClose;
            kcpServer.OnException += KcpServer_OnException;

            Log.Info(string.Format("KCP-Server@{0}", this.LocalAddress.ToString()));
            return kcpServer;
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

        protected void OnReceiveBuffer(NetPeer peer, IByteBuffer buffer)
        {
            if(peer.networkType == NetworkType.TCP)
            {

            }
            //Ping/Pong msg process 
            if (buffer.ReadableBytes == 1)
            {
                byte protoCode = buffer.ReadByte();
                if (protoCode == (byte)OpCode.PING)
                {
                    peer.Send(new byte[] { (byte)OpCode.PONG });
                }
                else if (protoCode == (byte)OpCode.GOODBYE)
                {
                    //删除这个连接
                    NetManager.Instance.Deregister(peer);
                    peer.Send(new byte[] { (byte)OpCode.GOODBYE });
                }

                return;
            }
            else
            {
                uint protoCode = buffer.ReadUnsignedIntLE();
                if (protoCode >= (uint)OpCode.CALL_ACTOR_METHOD)
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    uint fromActorId = buffer.ReadUnsignedIntLE();
                    uint toActorId = buffer.ReadUnsignedIntLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);

                    //var msg = MessagePackSerializer.Deserialize<ActorMessage>(bytes);
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, fromActorId, toActorId, peer.networkType, bytes);
                    CallActorMethod(peer.ConnId, packet);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    uint fromHostId = buffer.ReadUnsignedIntLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    if(peer.ConnId != fromHostId)
                    {
                        //修正一下peer的id 
                        NetManager.Instance.ChangePeerId(peer.ConnId, fromHostId);
                    }
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Host.Instance.Id, 0, 0, peer.networkType, bytes);
                    this.CallMethod(peer.ConnId, this.Id, packet);
                }
            }
        }

        private void KcpServer_OnReceive(Ukcp ukcp, IByteBuffer buffer)
        {
            var peer = NetManager.Instance.GetPeer(ukcp);
            OnReceiveBuffer(peer, buffer);  
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
            Log.Info(string.Format("TCP-Server@{0}", this.LocalAddress.ToString()));
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

            Console.WriteLine(peer.RemoteAddress + "|" + channel.RemoteAddress.ToString() +"=>" + StringUtil.ToHexString(buffer.ToArray()));

            OnReceiveBuffer(peer, buffer);
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
            clientPeer?.Send(new byte[] { (byte)OpCode.PING });
        }

        protected void Register()
        {

        }

        private void Server_OnException(NetPeer peer, Exception ex)
        {
            NetManager.Instance.Deregister(peer);
            Console.WriteLine(ex.StackTrace);
            peer.Stop();
            clientPeer = null;
            this.LocalAddress = null;
        }

        private void Server_OnClose(NetPeer peer)
        {
            NetManager.Instance.Deregister(peer);
            peer.Stop();
            clientPeer = null;
            this.LocalAddress = null;
        }

        private void Server_OnReceive(NetPeer peer, IByteBuffer buffer)
        {
            Console.WriteLine(peer.RemoteAddress + "=>"+StringUtil.ToHexString(buffer.ToArray()));
            OnReceiveBuffer(peer, buffer); 
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
            Log.Info(string.Format("CreateActor:success {0} {1}", name, newActor.Id));
            return newActor;
        }
        
        public Actor CreateActor(string typename, string name) 
        {
            var type = Global.TypeManager.Get(typename);
            var newActor = Actor.Create(type, name);
            Log.Info(string.Format("CreateActor:success {0} {1}", name, newActor.Id));
            return CreateActor(newActor);
        }

        public Actor CreateActor(Actor actor)
        {
            this.RegisterGlobalManager(actor); 
            actorDic[actor.Id] = actor;
            return actor;
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
            if(packet.ToActorId == 0)
            {
                this.CallMethod(fromHostId, packet.ToHostId, packet);
                return;
            }
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

        //public dynamic GetService(string name)
        //{
        //    return Global.GetActorRef(name, null);
        //}

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, null, Host.Instance);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), uid, null, Host.Instance);
        }

        //public ActorRef GetActorRef(string name)
        //{
        //    return Global.GetActorRef(name, null);
        //}

        public T GetActorRef<T>(string name) where T: ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, null, Host.Instance);
        }

        public T GetService<T>() where T : ActorRef
        {
            var refTypeName = typeof(T).Name;
            string name = refTypeName.Substring(0, refTypeName.Length - 3); 
            return (T)Global.GetActorRef(typeof(T), name, null, Host.Instance);
        }

        public T GetService<T>(string hostName, string ip, int port) where T : ActorRef
        {
            var refTypeName = typeof(T).Name;
            string name = refTypeName.Substring(0, refTypeName.Length - 3);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            return (T)Global.GetActorRefByAddr(typeof(T), ep, hostName, name,  null, Host.Instance);
        }
    }
}
