using Fenix;
using Fenix.Common;
using Fenix.Common.Utils;
using Fenix.Config;
using MessagePack.Resolvers;
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
            var app = new App();
            app.Init(null);
            app.Login("sekkit", "password", (code, avatar) =>
            {
                
            });
        } 
#endif
        public void Init(RuntimeConfig cfg, bool threaded = true)
        { 
            StaticCompositeResolver.Instance.Register(
                 MessagePack.Resolvers.ClientAppResolver.Instance,
                 MessagePack.Resolvers.FenixRuntimeResolver.Instance,
                 MessagePack.Resolvers.SharedResolver.Instance,
                 MessagePack.Unity.UnityResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );

            Environment.SetEnvironmentVariable("AppName", "Client.App");
            Global.Init(cfg, new Assembly[] { typeof(App).Assembly });
            host = Host.CreateClient();
            if (threaded)
                HostHelper.RunThread(host);
            else
            {
#if !CLIENT
                HostHelper.Run(host);
#else

#endif
            }
        }

        public void Login(string username, string password, Action<ErrCode, Avatar> callback)
        {
            //var localAddr = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
            //localAddr = "182.254.179.250"; 
            Log.Info("Connecting", Global.Config.HostIP, Global.Config.Port);
            var loginapp = host.GetHost("Login.App", Global.Config.HostIP, Global.Config.Port);
            //注册客户端，初始化路由表信息 

            loginapp.RegisterClient(host.Id, host.UniqueName, (code, hostInfo) =>
            {
                if (code == DefaultErrCode.ERROR)
                {
                    Log.Error("register_client_error, plz try again later");
                    loginapp.Disconnect();
                    callback?.Invoke(ErrCode.ERROR, null);
                    return;
                }

                Log.Info(string.Format("Register to server {0}: {1} {2} {3}", code,
                    hostInfo.HostId, hostInfo.HostName, hostInfo.HostAddr));

                if (loginapp.toHostId != hostInfo.HostId)
                    Global.NetManager.ChangePeerId(loginapp.toHostId, hostInfo.HostId, hostInfo.HostName, hostInfo.HostAddr);

                Global.IdManager.RegisterHostInfo(hostInfo);

                if (code == 0)
                {
                    //发起登陆请求，得到玩家entity所在host信息
                    Log.Info("Request Login", username, password);
                    var svc = host.GetService<LoginServiceRef>();
                    svc.rpc_login(username, password, (code2, uid, hostId, hostName, hostAddress) =>
                    {
                        if (code2 != ErrCode.OK)
                        {
                            Log.Error("login_failed");
                            loginapp.Disconnect();
                            callback?.Invoke(code2, null);
                            return;
                        }

                        Log.Info(string.Format("ServerAvatar host: {0}@{1} {2} {3}", uid, hostId, hostName, hostAddress));
                        var avatar = host.CreateActorLocally<Client.Avatar>(uid);

                        Global.IdManager.RegisterHost(hostId, hostName, hostAddress, hostAddress);
                        Global.IdManager.RegisterActor(avatar, hostId);

                        var parts = hostAddress.Split(':');
                        var ip = parts[0];
                        var port = int.Parse(parts[1]);
                        var avatarHost = host.GetHost(hostName, ip, port);
                        Global.NetManager.PrintPeerInfo("# Master.App: hostref created");
                        avatarHost.BindClientActor(avatar.Uid, (code3) =>
                        {
                            Global.NetManager.PrintPeerInfo("# Master.App: BindClientActor called");
                            Log.Info("Avatar已经和服务端绑定");

                            callback?.Invoke((ErrCode)code3, avatar);
                        });
                        loginapp.Disconnect();
                    });
                }
            }); 
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
            host = null;
        }
    }
}
