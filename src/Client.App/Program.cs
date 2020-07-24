using Fenix;
using Fenix.Common.Utils;
using MessagePack;
using Server;
using System;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Init(new Assembly[] { typeof(Program).Assembly });

            var localAddr = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);

            /*
             * Client是单点，还是多点呢，思考一下
             */

            var host = Host.CreateClient();  
            if (host == null)
            {
                Console.WriteLine(string.Format("unable_connect_to_server {0}:{1}", localAddr, 17777));
                return;
            }

            //var svc = host.GetService<LoginServiceRef>("Login.App", localAddr, 17777);

            var loginapp = host.GetHost("Login.App", localAddr, 17777);
            //注册客户端，初始化路由表信息
            loginapp.RegisterClient(host.Id, host.UniqueName, (code) =>
            {
                Console.WriteLine(string.Format("Register to server {0}", code));

                var svc = host.GetService<LoginServiceRef>();
                svc.rpc_login("username", "password", (code, uid, hostId, hostName, hostAddress) =>
                {
                    Game.Avatar = host.CreateActor<Client.Avatar>(uid);
                    host.GetHost(hostName, hostAddress, 0).BindClientActor(Game.Avatar.Uid, (code2) =>
                    {

                    });

                    //host.GetActorRef<Server.AvatarRef>(uid).rpc_bind_client(uid);
                });
            });


            
            HostHelper.Run(host);
        }
    }
}
