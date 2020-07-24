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
 
            var host = Host.CreateClient();  
            if (host == null)
            {
                Console.WriteLine(string.Format("unable_connect_to_server {0}:{1}", localAddr, 17777));
                return;
            } 

            var loginapp = host.GetHost("Login.App", localAddr, 17777);
            //注册客户端，初始化路由表信息
            loginapp.RegisterClient(host.Id, host.UniqueName, (code, hostInfo) =>
            {
                Console.WriteLine(string.Format("Register to server {0}", code));
                Global.IdManager.RegisterHostInfo(hostInfo);

                if(code == 0)
                {
                    //发起登陆请求，得到玩家entity所在host信息
                    var svc = host.GetService<LoginServiceRef>();
                    svc.rpc_login("username", "password", (code, uid, hostId, hostName, hostAddress) =>
                    {
                        Game.Avatar = host.CreateActor<Client.Avatar>(uid);
                        var parts = hostAddress.Split(':');
                        var ip = parts[0];
                        var port = int.Parse(parts[1]);
                        host.GetHost(hostName, ip, port).BindClientActor(Game.Avatar.Uid, (code2) =>
                        {
                            Console.WriteLine("Avatar已经和服务端绑定");
                        });
                    });
                }
            });
            
            HostHelper.Run(host);
        } 
    }
}
