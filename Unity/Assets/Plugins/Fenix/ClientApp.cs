
using Fenix;
using Fenix.Common;
using Fenix.Common.Utils;
using Fenix.Config;
using Server;
using Shared;
using Shared.Protocol;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ClientApp : MonoBehaviour
    {
        public string HostIp = "127.0.0.1";

        public int Port = 17777;

        public NetworkType NetType = NetworkType.TCP;

        public void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            var cfg = RuntimeConfig.MakeDefaultClientConfig();
            cfg.AppName = "Client.App";
            cfg.HostIP = this.HostIp;
            cfg.Port = this.Port;
            cfg.ClientNetwork = this.NetType;
            App.Instance.Init(cfg, threaded:false);
        }

        public void Update()
        {
            App.Instance.Update();
        }

        public void OnDestroy()
        {
            App.Instance.OnDestroy();
            App.Instance = null;
        }

        public void Login(string userName, string password, Action<ErrCode, Avatar> callback)
        {
            App.Instance.Login(userName, password, callback);
        }

        public void Register(string userName, string password, bool isGuest, Action<ErrCode> callback)
        {
            //App.Instance.Register(userName, password, callback);
        }

        public void Logout(string userName, string password, Action<ErrCode> callback)
        {
            //App.Instance.Login(userName, password, callback);
        }

        public void ReInit()
        {
             
        }
    }
}