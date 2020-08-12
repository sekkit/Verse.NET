
using Shared.Protocol;
using System;
using System.Threading.Tasks; 
using UnityEngine;

namespace Client
{
    public class ClientApp : MonoBehaviour
    {
        public void Start()
        {
            DontDestroyOnLoad(this.gameObject); 
            App.Instance.Init(threaded:false);
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