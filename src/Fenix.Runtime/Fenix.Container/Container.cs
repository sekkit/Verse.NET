// 

using System;
using System.Threading;
using DotNetty.KCP;
using DotNetty.Buffers;
using Fenix.Global;
using DotNetty.Common.Utilities;
using System.Text;

namespace Fenix
{
    public class Container
    {
        public uint InstanceId { get; set; }

        public string Tag { get; set; }

        protected KcpContainerServer kcpServer { get; set; }

        protected KcpContainerClient kcpClient { get; set; }

        protected string ip { get; set; }

        protected int port { get; set; }

        private ulong count = 0;

        protected TcpContainerServer tcpServer = new TcpContainerServer();

        protected TcpContainerClient tcpClient = new TcpContainerClient();

        protected Container(string _ip, int _port)
        {
            this.ip = _ip;
            this.port = _port; 

            this.RegisterToIdManager(this);

            this.SetupKcpServer();

            this.SetupKcpClient();

            this.SetupTcpServer();

            this.SetupTcpClient();

            kcpClient.Send(Encoding.UTF8.GetBytes("hello"));
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

        protected void SetupKcpClient()
        {
            kcpClient = KcpContainerClient.Create(this.ip, this.port);
            kcpClient.OnReceive += KcpClient_OnReceive;
            kcpClient.OnClose += KcpClient_OnClose;
            kcpClient.OnException += KcpClient_OnException;
            kcpClient.Send(Encoding.UTF8.GetBytes("1254124124"));
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
            count++;
            Console.WriteLine("FROM_CLIENT:" + data + " => " + count.ToString());
            // kcpServer.Send(buffer);  
            ukcp.writeMessage(Unpooled.WrappedBuffer(bytes));
            
            //ukcp.writeKcpMessage(buffer);
        } 
        private void KcpServer_OnException(Exception arg1, Ukcp arg2)
        {
            
        }

        private void KcpServer_OnClose(Ukcp obj)
        {
            
        }

        private void KcpClient_OnReceive(byte[] bytes, Ukcp sender)
        {
            //var bytes = new byte[buffer.ReadableBytes];
            //buffer.GetBytes(0, bytes);
            string data = StringUtil.ToHexString(bytes);
            //string data2 = buffer.GetString(0, buffer.ReadableBytes, Encoding.UTF8);
            Console.WriteLine("FROM_SERVER:"+data + " => " + Encoding.UTF8.GetString(bytes));
            sender.send(bytes);
        }
        private void KcpClient_OnException(Exception arg1, Ukcp arg2)
        {

        }

        private void KcpClient_OnClose(Ukcp obj)
        {

        }

        #endregion

        #region TCP
        protected void SetupTcpServer()
        {
            //tcpServer = TcpContainerServer.Create(this.ip, this.port);
            // tcpServer.OnReceive += TcpServer_OnReceive;
            // tcpServer.OnClose += TcpServer_OnClose;
            // tcpServer.OnException += TcpServer_OnException;
        }

        protected void SetupTcpClient()
        {
            kcpClient = KcpContainerClient.Create(this.ip, this.port);
            kcpClient.OnReceive += KcpClient_OnReceive;
            kcpClient.OnClose += KcpClient_OnClose;
            kcpClient.OnException += KcpClient_OnException;
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
