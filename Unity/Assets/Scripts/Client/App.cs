using DotNetty.KCP;
using Fenix;
using Fenix.Common;
using Fenix.Common.Utils;
using Server;
using Shared.Protocol;
using System;
using System.Net;
using System.Reflection; 

namespace Client
{ 
    public class App
    { 
        public static App Instance = new App();

        Host host;

#if !UNITY_5_3_OR_NEWER
        static void Main(string[] args)
        {
            new App().Init();
        }
#else
        private void Start()
        {
            this.Init();
        }
#endif
        public void Init(bool threaded=true)
        {
            Environment.SetEnvironmentVariable("AppName", "Client.App");
            Global.Init(new Assembly[] { typeof(App).Assembly });
            host = Host.CreateClient();
            if (threaded)
                HostHelper.RunThread(host);
            else
            {
#if !UNITY_5_3_OR_NEWER
                HostHelper.Run(host);
#else
                
#endif
            }
        }

        public void Login(string userName, string password, Action<ErrCode> callback)
        { 
            var localAddr = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
            //localAddr = "182.254.179.250"; 

            var loginapp = host.GetHost("Login.App", localAddr, 17777);
            //注册客户端，初始化路由表信息 

            loginapp.RegisterClient(host.Id, host.UniqueName, (code, hostInfo) =>
            {
                if (code == DefaultErrCode.ERROR)
                {
                    Log.Error("register_client_error, plz try again later");
                    loginapp.Disconnect();
                    callback?.Invoke(ErrCode.ERROR);
                    return;
                }

                Log.Info(string.Format("Register to server {0}: {1} {2} {3}", code, 
                    hostInfo.HostId, hostInfo.HostName, hostInfo.HostAddr));

                if (loginapp.toHostId != hostInfo.HostId)
                    NetManager.Instance.ChangePeerId(loginapp.toHostId, hostInfo.HostId, hostInfo.HostName, hostInfo.HostAddr);

                Global.IdManager.RegisterHostInfo(hostInfo); 

                if (code == 0)
                {
                    //发起登陆请求，得到玩家entity所在host信息
                    var svc = host.GetService<LoginServiceRef>();
                    svc.rpc_login("username", "password", (code2, uid, hostId, hostName, hostAddress) =>
                    {
                        if (code2 != ErrCode.OK)
                        {
                            Log.Error("login_failed");
                            loginapp.Disconnect();
                            callback?.Invoke(code2);
                            return;
                        }

                        Log.Info(string.Format("ServerAvatar host: {0}@{1} {2} {3}", uid, hostId, hostName, hostAddress));
                        Game.Avatar = host.CreateActor<Client.Avatar>(uid);

                        Global.IdManager.RegisterHost(hostId, hostName, hostAddress, hostAddress);
                        Global.IdManager.RegisterActor(Game.Avatar, hostId);

                        var parts = hostAddress.Split(':');
                        var ip = parts[0];
                        var port = int.Parse(parts[1]);
                        var avatarHost = host.GetHost(hostName, ip, port);
                        NetManager.Instance.PrintPeerInfo("# Master.App: hostref created");
                        avatarHost.BindClientActor(Game.Avatar.Uid, (code3) =>
                        {
                            NetManager.Instance.PrintPeerInfo("# Master.App: BindClientActor called");
                            Log.Info("Avatar已经和服务端绑定");

                            callback?.Invoke((ErrCode)code3);
                        });
                        loginapp.Disconnect();
                    });
                }
            });

            HostHelper.RunThread(host);
        }

        public void Close()
        {
            HostHelper.Stop(host);
        }

        public void Update()
        {
            HostHelper.Update(host);
        }

        public void OnDestroy()
        {
            this.Close();
        }
    }
}
