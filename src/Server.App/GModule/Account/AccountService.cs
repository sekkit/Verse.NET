using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using static Fenix.Common.RpcUtil;

namespace GModule.Login
{
    public partial class AccountService : Actor
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
            //if (sent) return;
            //sent = true;
            var svc = GetService("MatchService");
            svc?.rpc_join_match("", i++, new Action<uint>((code) =>
            {
                Log.Info(code.ToString());
            }));
        }
    }
}
