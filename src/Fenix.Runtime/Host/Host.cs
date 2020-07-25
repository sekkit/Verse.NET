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
using Fenix;
using Fenix.Common.Utils; 
using Fenix.Common.Attributes;
using System.Threading;
using System.Reflection.PortableExecutable;
using Shared.Protocol;
using TimeUtil = Fenix.Common.Utils.TimeUtil;
using System.Text;

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

        //protected NetPeer clientPeer { get; set; }

        public bool IsClientMode { get; set; }

        protected ConcurrentDictionary<UInt32, Actor> actorDic = new ConcurrentDictionary<UInt32, Actor>();
      
        private ConcurrentDictionary<ulong, Timer> mTimerDic = new ConcurrentDictionary<ulong, Timer>();


        protected Host(string name, string ip, int port = 0, bool clientMode = false) : base()
        {
            this.IsClientMode = clientMode;

            //NetManager.Instance.OnConnect += (peer) => 
            NetManager.Instance.OnReceive += (peer, buffer) => OnReceiveBuffer(peer, buffer);
            NetManager.Instance.OnClose += (peer) => { };
            NetManager.Instance.OnException += (peer, ex) =>
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            };

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
                    this.UniqueName = Basic.GenID64().ToString();
                else
                    this.UniqueName = name;

                this.Id = Basic.GenID32FromName(this.UniqueName);

                this.RegisterGlobalManager(this);

                this.SetupKcpServer();
                this.SetupTcpServer();
            }
            else
            {
                //clientPeer = NetManager.Instance.CreatePeer(ip, port, NetworkType.KCP);
                //if (clientPeer != null)
                //{
                //    if (name == null)
                //        this.UniqueName = Basic.GenID64().ToString();
                //    else
                //        this.UniqueName = name;

                //    this.Id = Basic.GenID32FromName(clientPeer.LocalAddress.ToString());

                //    this.LocalAddress = clientPeer.LocalAddress;

                //    var thread = new Thread(new ThreadStart(Heartbeat));//创建线程 
                //    thread.Start();
                //}
                //else
                //{
                //    throw new Exception("unable_to_connect_server");
                //}

                if (name == null)
                    this.UniqueName = Basic.GenID64().ToString();
                else
                    this.UniqueName = name;
                this.Id = Basic.GenID32FromName(this.UniqueName); 
            }

            if (!this.IsClientMode)
            {
                Log.Info(string.Format("{0}(ID:{1}) is running at {2} as ServerMode", this.UniqueName, this.Id, LocalAddress.ToString()));
            }
            else
            {
                Log.Info(string.Format("{0}(ID:{1}) is running as ClientMode", this.UniqueName, this.Id));
                //Log.Info(string.Format("{0} is running at {1} as ClientMode", this.UniqueName, LocalAddress.ToString()));
            }

            var thread = new Thread(new ThreadStart(Heartbeat));
            thread.Start();
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
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
            return null;
        }

        public static Host CreateClient()//string ip, int port)
        {
            return Create(null, "", 0, true);
            //return Create(Basic.GenID32FromName(string.Format("{0}:{1}", ip, port)).ToString(), ip, port, true);
        }

        public static Host CreateServer(string name, string ip, int port)
        {
            return Create(name, ip, port, false);
        }

        protected void OnReceiveBuffer(NetPeer peer, IByteBuffer buffer)
        {
            
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
                if(protoCode == OpCode.REGISTER_REQ)
                {
                    var hostId = buffer.ReadUnsignedIntLE();
                    var nameBytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(nameBytes);
                    var hostName = Encoding.UTF8.GetString(nameBytes);
                     
                    var context = new RpcContext(null, peer);

                    this.Register(hostId, hostName, context);

                    return;
                }
                //if (protoCode >= (uint)OpCode.CALL_ACTOR_METHOD)
                //{
                
                ulong msgId = (ulong)buffer.ReadLongLE();
                //uint fromHostId = buffer.ReadUnsignedIntLE();
                uint fromActorId = buffer.ReadUnsignedIntLE();
                uint toActorId = buffer.ReadUnsignedIntLE();
                byte[] bytes = new byte[buffer.ReadableBytes];
                buffer.ReadBytes(bytes);
                //if (peer.ConnId != fromHostId)
                //{
                //    //修正一下peer的id 
                //    NetManager.Instance.ChangePeerId(peer.ConnId, fromHostId);
                //} 

                //var msg = MessagePackSerializer.Deserialize<ActorMessage>(bytes); 
                var packet = Packet.Create(msgId, 
                    protoCode, 
                    peer.ConnId, 
                    Host.Instance.Id,
                    fromActorId, 
                    toActorId, 
                    peer.networkType, 
                    Global.TypeManager.GetMessageType(protoCode), 
                    bytes);

                Console.WriteLine(string.Format("RECV({0}): {1} {2} => {3} {4} >= {5} {6} => {7}", peer.networkType, protoCode, packet.FromHostId, packet.ToHostId,
                    packet.FromActorId, packet.ToActorId, peer.RemoteAddress.ToString(), peer.LocalAddress.ToString()));
                
                //Console.WriteLine(string.Format("RECV{0} {1}", packet.ToString()));


                if (protoCode >= OpCode.CALL_ACTOR_METHOD && toActorId != 0)
                {
                    this.CallActorMethod(packet);
                }
                else
                {
                    this.CallMethod(packet);
                } 
            }
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
        #endregion

        #region TCP
        protected TcpHostServer SetupTcpServer()
        {
            tcpServer = TcpHostServer.Create(this.LocalAddress);
            tcpServer.Connect += OnTcpConnect;
            tcpServer.Receive += OnTcpServerReceive;
            tcpServer.Close += OnTcpServerClose;
            tcpServer.Exception += OnTcpServerException;
            Log.Info(string.Format("TCP-Server@{0}", this.LocalAddress.ToString()));
            return tcpServer;
        }
         
        void OnTcpConnect(IChannel channel)
        {
            //新连接
            NetManager.Instance.RegisterChannel(channel);
            //ulong hostId = Global.IdManager.GetHostId(channel.RemoteAddress.ToString());
            Console.WriteLine("TcpConnect: " + channel.RemoteAddress.ToString());
        }

        void OnTcpServerReceive(IChannel channel, IByteBuffer buffer)
        {
            var peer = NetManager.Instance.GetPeer(channel);
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
                if (IsClientMode)
                {
                    NetManager.Instance.Ping();
                }
                else
                {
                    this.RegisterGlobalManager(this);
                    foreach (var kv in this.actorDic)
                        this.RegisterGlobalManager(kv.Value);
                }
                Thread.Sleep(5000);
            }
        }

        protected void Ping(NetPeer clientPeer)
        {
            clientPeer?.Send(new byte[] { (byte)OpCode.PING });
        }

        protected void RegisterGlobalManager(Host host)
        {
            Global.IdManager.RegisterHost(host, this.LocalAddress.ToString());
        }

        protected void RegisterGlobalManager(Actor actor)
        {
            Global.IdManager.RegisterActor(actor, this.Id);
            Global.TypeManager.RegisterActorType(actor);
        }

        public Actor GetActor(uint actorId)
        {
            if (this.actorDic.TryGetValue(actorId, out Actor a))
                return a;
            return null;
        }

        [ServerOnly]
        public void CreateActor(string typename, string name, Action<DefaultErrCode, string, uint> callback, RpcContext __context)
        {
            var a = CreateActor(typename, name);

            if (a != null)
                callback(DefaultErrCode.OK, a.UniqueName, a.Id);
            else
                callback(DefaultErrCode.ERROR, "", 0);
        }

        public T CreateActor<T>(string name) where T : Actor
        {
            var newActor = Actor.Create(typeof(T), name);
            this.ActivateActor(newActor);
            Log.Info(string.Format("CreateActor:success {0} {1}", name, newActor.Id));
            return (T)newActor;
        }

        public Actor CreateActor(string typename, string name)
        {
            var type = Global.TypeManager.Get(typename);
            var newActor = Actor.Create(type, name);
            Log.Info(string.Format("CreateActor:success {0} {1}", name, newActor.Id));
            ActivateActor(newActor);
            return newActor;
        }

        public Actor ActivateActor(Actor actor)
        {
            this.RegisterGlobalManager(actor);
            actor.onLoad();
            actorDic[actor.Id] = actor;
            return actor;
        }

        //迁移actor
        [ServerOnly]
        public void MigrateActor(uint actorId, RpcContext __context)
        {

        }

        [ServerOnly]
        //移除actor
        public void RemoveActor(uint actorId, RpcContext __context)
        {

        }

        [ServerApi]
        public void Register(uint hostId, string hostName, RpcContext __context)
        {
            if (__context.Peer.ConnId != hostId)
            {
                //修正一下peer的id 
                NetManager.Instance.ChangePeerId(__context.Peer.ConnId, hostId, hostName, __context.Peer.RemoteAddress.ToString()); 
            }
            else
            {
                Global.IdManager.RegisterHost(hostId, hostName, __context.Peer.RemoteAddress.ToString());
            }
        }

#if !CLIENT

        [ServerApi]
        public void RegisterClient(uint hostId, string hostName, Action<int, HostInfo> callback, RpcContext __context)
        {
            if (__context.Peer.ConnId != hostId)
            {
                NetManager.Instance.ChangePeerId(__context.Peer.ConnId, hostId, hostName, __context.Peer.RemoteAddress.ToString());
            }

            NetManager.Instance.RegisterClient(hostId, hostName, __context.Peer);

            var hostInfo = Global.IdManager.GetHostInfo(this.Id);

            callback((int)DefaultErrCode.OK, hostInfo);
        }

        [ServerApi]
        public void BindClientActor(string actorName, Action<DefaultErrCode> callback, RpcContext __context)
        {
            //首先这个actor一定是本地的
            //如果actor不在本地，则把请求转到目标host上去

            //find actor.server
            var actorId = Global.IdManager.GetActorId(actorName);
            //var hostAddr = Global.IdManager.GetHostAddrByActorId(actorId, false);
            Global.IdManager.RegisterClientActor(actorId, actorName, __context.Packet.FromHostId, __context.Peer.RemoteAddress.ToString());
             
            //give actor.server hostId, ipaddr to client
            callback(DefaultErrCode.OK);

            //Set actor.server's client property
            var a = Host.Instance.GetActor(actorId);
            a.OnClientEnable(actorName);
        }
#endif
        //调用Actor身上的方法
        protected void CallActorMethod(Packet packet)  
        {
            if(packet.ToActorId == 0)
            {
                this.CallMethod(packet);
                return;
            }

            var actor = this.actorDic[packet.ToActorId]; 
            actor.CallMethod(packet);
        }

        public sealed override void Update()
        {
            //Log.Info(string.Format("{0}:{1}", this.GetType().Name, rpcDic.Count));
            var curTime = TimeUtil.GetTimeStampMS();

            var keys = this.mTimerDic.Keys;

            foreach (var key in keys)
            {
                if (this.mTimerDic.TryGetValue(key, out var t))
                {
                    if (!t.CheckTimeout(curTime))
                    {
                        this.mTimerDic.TryRemove(key, out var _);
                        t.Dispose();
                    }
                }
            }

            foreach (var a in this.actorDic.Keys)
            {
                this.actorDic[a].Update();
            }

            NetManager.Instance.Update();

            //Log.Info(string.Format("C: {0}", rpcDic.Count));
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, null, Host.Instance);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), uid, null, Host.Instance);
        } 
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

        //public T GetService<T>(string hostName, string ip, int port) where T : ActorRef
        //{
        //    var refTypeName = typeof(T).Name;
        //    string name = refTypeName.Substring(0, refTypeName.Length - 3);
        //    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        //    return (T)Global.GetActorRefByAddr(typeof(T), ep, hostName, name,  null, Host.Instance);
        //}

        public ActorRef GetHost(string hostName, string ip, int port)
        { 
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            return Global.GetActorRefByAddr(typeof(ActorRef), ep, hostName, "", null, Host.Instance);
        }

        public void AddTimer(long delay, long interval, Action tickCallback)
        {
            //实现timer
            var timer = Timer.Create(delay, interval, false, tickCallback);
            this.mTimerDic.TryAdd(timer.Tid, timer);
        }

        public void AddRepeatedTimer(long delay, long interval, Action tickCallback)
        {
            //实现timer
            var timer = Timer.Create(delay, interval, true, tickCallback);
            //this.mTimerDic[timer.Tid] = timer;
            this.mTimerDic.TryAdd(timer.Tid, timer);
        }
    }
}
