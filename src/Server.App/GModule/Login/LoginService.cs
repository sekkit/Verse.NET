 
using DotNetty.KCP;
using DotNetty.Transport.Libuv.Native;
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

        //callback: code, actorName, actorHostId, actorHostName, actorHostAddress, 
        [ServerApi]
        public void Login(string username, string password, Action<ErrCode, string, uint, string, string> callback)
        {
            Console.WriteLine(string.Format("login {0} {1}", username, password));

            //验证用户db，成功则登陆
            //服务端生成玩家avatar
            //通常是在MasterService上，生成玩家，注意玩家可以随意迁移

            //分配玩家uid
            //分配玩家游客名称 
            var uid = Global.DbManager.CreateUid();

            var svc = GetService<MasterServiceRef>();
            svc.CreateActor(nameof(Avatar), uid, (code, actorName, actorId) =>
            {
                //创建成功后，把客户端的avatar注册到服务端
                Log.Info(string.Format("login.create_actor@Master.App {0}", code));

                //if(code == DefaultErrCode.OK)
                //{
                //    GetAvatar<Client.AvatarRef>(uid).client_on_api_test("", 1, (c)=> {
                //        Log.Info(string.Format("login->create_actor {0}", c));
                //    });
                //    callback(ErrCode.OK, uid);
                //}
                //Global.IdManager.GetHostIdByActorId();

                var hostId = Global.IdManager.GetHostIdByActorId(actorId, false);
                ErrCode retCode = (code == DefaultErrCode.OK ? ErrCode.OK : ErrCode.ERROR);
                callback(
                    retCode,
                    actorName,
                    hostId,
                    Global.IdManager.GetHostName(hostId),
                    Global.IdManager.GetHostAddrByActorId(actorId, false)
                );
            });
        }

        [ServerApi]
        public void ResetPassword(string username, string email)
        {

        }

        //bool sent = false;

        int i = 0;
    }
}
