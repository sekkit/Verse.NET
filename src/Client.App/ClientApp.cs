using System;
using Fenix;
using Fenix.Common;
using Fenix.Common.Utils; 
using System.Net;
using DotNetty.Buffers;
using Shared.Protocol.Message;
using System.Threading;
using System.Text.Unicode;
using System.Text;
using DotNetty.Common.Utilities;

namespace Client.App
{
    //Client一般是单点的
    //
    public class ClientApp : RpcModule
    {
        public string Ip = "127.0.0.1";

        public int Port = 17777; 

        protected NetPeer clientPeer;
        
        public static ClientApp Create()
        {
            return new ClientApp();
        }

        protected ClientApp(bool isTcp=false)
        {
            //创建一个客户端的Container
            //用这个Container和AccountService通信
            //一旦登陆完，avatar创建完成，就可以直接用avatar通信了
            //var c = Container.Create(this.Ip, this.Port); 
            //ContainerHelper.Run(c);

            //server = NetManager.Instance.CreatePeer(this.Ip, this.Port);

            clientPeer = NetPeer.Create(this.Ip, this.Port);
            clientPeer.OnReceive += Server_OnReceive;
            clientPeer.OnClose += Server_OnClose;
            clientPeer.OnException += Server_OnException;

            Thread thread = new Thread(new ThreadStart(Heartbeat));//创建线程 
            thread.Start();                                                           //启动线程
        }

        protected void Heartbeat()
        {
            while(true)
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

        private void Server_OnReceive(NetPeer arg1, IByteBuffer arg2)
        {
            
            Console.WriteLine(StringUtil.ToHexString(arg2.ToArray()));
        }

        public void Login(string username, string password, Action<DefaultErrCode> callback)
        {
            var req = new LoginReq();

            req.username = "";
            req.password = "";

            ulong msgId = Basic.GenID64();
            var packet = Packet.Create(msgId, ProtocolCode.LOGIN_REQ, 0, 0,  0, Basic.GenID32FromName("AccountService"), RpcUtil.Serialize(req));

            clientPeer.Send(packet.Pack());
        }
    }

}
