// 

using System;
using System.Collections.Generic;
using DotNetty.KCP;
using DotNetty.Buffers;
using Fenix.Global;
using DotNetty.Common.Utilities;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Fenix.Fenix.Container;

namespace Fenix
{
    public class Container
    {
        public uint InstanceId { get; set; }

        public string Tag { get; set; }

        protected KcpContainerServer kcpServer { get; set; }

        //protected KcpContainerClient kcpClient { get; set; }
        
        protected Dictionary<uint, NetPeer> peers { get; set; }
        
        protected Dictionary<uint, TcpContainerClient> tcpClientDic { get; set; }
        
        protected Dictionary<uint, KcpContainerClient> kcpClientDic { get; set; }

        protected string ip { get; set; }

        protected int port { get; set; }

        private ulong count = 0;

        protected TcpContainerServer tcpServer = new TcpContainerServer();

        //protected TcpContainerClient tcpClient = new TcpContainerClient();
        
        protected Container(string _ip, int _port)
        {
            this.ip = _ip;
            this.port = _port;

            this.RegisterToIdManager(this);

            this.SetupKcpServer();

            //this.SetupKcpClient();

            this.InitTcp();

            //kcpClient.Send(Encoding.UTF8.GetBytes("hello"));
        }

        async Task InitTcp()
        {
            await this.SetupTcpServer();

            //await this.SetupTcpClient();
        }

        public static Container Create(string ip, int port)
        {
            return new Container(ip, port);
        }
        
        #region KCP

        protected void SetupKcpServer()
        {
            kcpServer = KcpContainerServer.Create(this.ip, this.port);
            kcpServer.OnReceive += KcpServer_OnReceive;
            kcpServer.OnClose += KcpServer_OnClose;
            kcpServer.OnException += KcpServer_OnException;
        }
 
        //private long last_ts = DateTime.Now.Ticks;
        
        protected KcpContainerClient CreateKcpClient()
        {
            var kcpClient = KcpContainerClient.Create(this.ip, this.port);
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
        protected async Task SetupTcpServer()
        {
             tcpServer = await TcpContainerServer.Create(this.ip, this.port);
             tcpServer.Connect += OnTcpIncomingConnect;
             tcpServer.Receive += OnTcpServerReceive;
             tcpServer.Close += OnTcpServerClose;
             tcpServer.Exception += OnTcpServerException;
        }

        protected async Task<TcpContainerClient> CreateTcpClient(string _ip, int _port)
        {
            var tcpClient = await TcpContainerClient.Create(_ip, _port);
            tcpClient.Receive += OnTcpClientReceive;
            tcpClient.Close += OnTcpClientClose;
            tcpClient.Exception += OnTcpClientException;
            return tcpClient;
            
            //tcpClient.Send(Encoding.UTF8.GetBytes("hello"));
            //Console.WriteLine("send:hello");
        }
        
        
        void OnTcpIncomingConnect(IChannel channel)
        {
            //新连接
            NetManager.Instance.RegisterChannel(channel);
            var peer = NetPeer.Create(channel);
            string chId = channel.Id.AsLongText();
            //重连
            
            
        }
        
        void OnTcpServerReceive(IChannel channel, IByteBuffer buffer)
        {
            var cur_ts = DateTime.Now.Ticks;
            //Console.WriteLine("FROM_CLIENT:" + buffer.ToString() + " => " + ((cur_ts - last_ts)/10000.0).ToString()); //stopWatch.Elapsed.TotalMilliseconds.ToString());
            last_ts = cur_ts;
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
            IdManager.Instance.RegisterContainer(container.InstanceId, string.Format("{0}:{1}", ip, port));
        }

        protected void RegisterToIdManager(Actor actor)
        {
            IdManager.Instance.RegisterActor(actor.InstanceId, this.InstanceId);
        }

        protected void SpawnActor<T>() where T: ActorLogic, new()
        {
            var newActor = Actor.Create<T>();
            this.RegisterToIdManager(newActor);
        }

        //迁移actor
        protected void MigrateActor(uint actorId)
        {

        }

        //移除actor
        protected void RemoveActor(uint actorId)
        {

        }

        //调用Actor身上的方法
        protected void CallActorMethod(uint actorId, uint methodId, object[] args)
        {

        }

        public virtual void Update()
        {

        } 
    }
}
