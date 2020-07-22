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

        public uint Id { get; set; }
        
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

            this.Id = Basic.GenID32FromName(clientPeer.LocalAddress.ToString());

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
                else if(protoCode == (byte)ProtoCode.PONG)
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
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Container.Instance.Id, fromActorId, toActorId, bytes);
                    
                    //HandleIncomingActorMessage(peer, packet);
                }
                else
                {
                    ulong msgId = (ulong)buffer.ReadLongLE();
                    byte[] bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    var packet = Packet.Create(msgId, protoCode, peer.ConnId, Container.Instance.Id, 0, 0, bytes);

                    this.CallMethod(peer.ConnId, this.Id, packet);
                }
            }

            Console.WriteLine(StringUtil.ToHexString(buffer.ToArray()));
        }

        //Test code
        public void Login(string username, string password, Action<DefaultErrCode> callback)
        {
            //var svc = this.GetService("AccountService");
            //svc.rpc_login(username, password, callback);

            var req = new LoginReq();

            req.username = "";
            req.password = "";

            ulong msgId = Basic.GenID64();
            var packet = Packet.Create(msgId, ProtocolCode.LOGIN_REQ, this.Id, 0,  0, Basic.GenID32FromName("AccountService"), RpcUtil.Serialize(req));
            
            clientPeer.Send(packet.Pack());
        }
    }

}
