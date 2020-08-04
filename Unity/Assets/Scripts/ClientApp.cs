using Client;
using DotNetty.KCP;
using Fenix;
using Fenix.Common;
using Fenix.Common.Utils;
using Server;
using Shared.Protocol;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Client
{
    public class ClientApp : MonoBehaviour
    {
        public void Start()
        { 
            App.Instance.Init(threaded:false);
        }

        public void Update()
        {
            App.Instance.Update();
        }

        public void OnDestroy()
        {
            App.Instance.OnDestroy();
        }

        public void Login(string userName, string password, Action<ErrCode> callback)
        {
            Task.Run(() => App.Instance.Login(userName, password, callback));
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