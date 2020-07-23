using DotNetty.KCP;
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Server.UModule;
using Shared;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using static Fenix.Common.RpcUtil;

namespace Server.GModule
{
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    public partial class LoginService : Service
    {
        public LoginService(string name): base(name)
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

            //验证用户db，成功则登陆
            //服务端生成玩家avatar
            //通常是在MasterService上，生成玩家，注意玩家可以随意迁移

            //分配玩家uid
            //分配玩家游客名称

            var uid = Global.DbManager.CreateUid();

            //actor创建后，必须绑定到host中才能正常运作 
            //var a = Actor.Create<Server.UModule.Avatar>(uid);

            GetService<MasterServiceRef>().CreateActor(nameof(Avatar), uid, (code)=> {
                callback(ErrCode.OK);
            });
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
        }
    }
}
