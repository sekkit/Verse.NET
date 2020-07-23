using DotNetty.KCP;
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using static Fenix.Common.RpcUtil;

namespace Server.GModule
{
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    public partial class AccountService : Service
    {
        public AccountService(string name): base(name)
        {
            
        }

        [ServerApi] 
        public void CreateAccount(string username, string password, string extra, Action<ErrCode> callback)
        {

        }


        [ServerApi]
        public void DeleteAccount(string username, string password, Action<ErrCode> callback)
        {

        }

        [ServerApi]
        public void Login(string username, string password, Action<ErrCode> callback)
        {
            Console.WriteLine(string.Format("login {0} {1}", username, password));
            callback(ErrCode.OK);
        }

        [ServerApi]
        public void ResetPassword(string username, string email)
        {

        }

        //bool sent = false;

        int i = 0;

        public override void Update()
        {
            base.Update();

            //var svc = GetActorRef<MatchServiceRef>("MatchService");
            //svc?.rp
            //var svc = GetService("MatchService");
            //svc?.rpc_join_match("", i++, new Action<ErrCode>((code) =>
            //{
            //    Log.Info(code.ToString());
            //}));
        }
    }
}
