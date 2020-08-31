//LoginService

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Redis;
using Server.Config;
using Server.DataModel;
using Server.UModule;
using Shared.DataModel;
using Shared.Protocol;
using System;
using System.Threading.Tasks;

namespace Server.GModule
{
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    public partial class LoginService : Service
    {
        private RedisDb LoginDb => Global.DbManager.GetDb(DbConfig.LOGIN);

        [ServerApi] 
        public void CreateAccount(string username, string password, string extra, Action<ErrCode> callback)
        {

        }

        [ServerApi]
        public void CreateAccount(string username, string password, Action<ErrCode> callback)
        {
            
        }

        [ServerApi]
        public void DeleteAccount(string username, string password, Action<ErrCode> callback)
        {

        } 

        //callback: code, actorName, actorHostId, actorHostName, actorHostAddress, 
        [ServerApi]
        public async Task Login(string username, string password, Action<ErrCode, string, ulong, string, string> callback)
        {
            Log.Info(string.Format("login {0} {1}", username, password));
            var loginData = LoginDb.Get<long>(username);
            if (loginData == -1)
            {
                callback(ErrCode.LOGIN_IN_PROGRESS, null, 0, null, null);
                return;
            }

            Log.Error(TimeUtil.GetTimeStampMS() - loginData);
            if (TimeUtil.GetTimeStampMS() - loginData < 3000)
            {
                callback(ErrCode.LOGIN_TOO_FREQ, null, 0, null, null);
                return;
            }

            LoginDb.Set(username, (long)-1, expireSec:3);

            //验证用户db，成功则登陆
            var account = AccountDb.Get<Account>(username);
            if (account != null)
            {
                if(account.password != password)
                {
                    callback(ErrCode.LOGIN_WRONG_USR_OR_PSW, null, 0, null, null);
                    LoginDb.Delete(username);
                    return;
                }
            } 
            else //如果不存在，则创建游客账号
            {
                account = new Account()
                {
                    username = username,
                    password = password,
                    uid = CreateUid()
                };

                if(!AccountDb.Set(username, account))
                {
                    callback(ErrCode.LOGIN_CREATE_ACCOUNT_FAIL, null, 0, null, null);
                    LoginDb.Delete(username);
                    return;
                }
            }

            //服务端生成玩家avatar
            //通常是在MasterService上，生成玩家，注意玩家可以随意迁移

            //如果已经存在了该actor，则直接找到它
            var actorId = Global.IdManager.GetActorId(account.uid);
            var hostId  = Global.IdManager.GetHostIdByActorId(actorId);
            if (hostId != 0)
            {
                var hostAddr = Global.IdManager.GetHostAddrByActorId(actorId);
                var clientId = Global.IdManager.GetHostIdByActorId(actorId, true);
                if(clientId != 0)
                {
                    //踢掉之前的客户端
                    var self = ActorRef(); 
                    var result = await self.RemoveClientActorAsync(actorId, DisconnectReason.KICKED);
                    if (result.code != DefaultErrCode.OK)
                    {
                        callback(
                            ErrCode.ERROR,
                            account.uid,
                            hostId,
                            Global.IdManager.GetHostName(hostId),
                            Global.IdManager.GetExtAddress(hostAddr));
                        LoginDb.Delete(username);
                        return;
                    }
                }

                Log.Info(string.Format("login.GetActorFrom@Master.App {0} {1} {2} {3}", account.uid, actorId,
                    Global.IdManager.GetHostName(hostId), hostAddr));

                callback(
                    ErrCode.OK,
                    account.uid,
                    hostId,
                    Global.IdManager.GetHostName(hostId),
                    Global.IdManager.GetExtAddress(hostAddr)
                );

                LoginDb.Set(username, TimeUtil.GetTimeStampMS(), expireSec: 3600);
                return;
            }

            //如果不存在，则申请创建一个
            var svc = GetService<MasterServiceRef>();
            svc.CreateActor(nameof(Avatar), account.uid, (code, actorName, actorId) =>
            {
                if (code != DefaultErrCode.OK)
                {
                    callback(ErrCode.ERROR, actorName, 0, null, null);
                    LoginDb.Delete(username);
                    return;
                }

                var hostId = Global.IdManager.GetHostIdByActorId(actorId); //, false);
                //创建成功后，把客户端的avatar注册到服务端
                var hostAddr = Global.IdManager.GetHostAddrByActorId(actorId);
                Log.Info(string.Format("login.create_actor@Master.App {0} {1} {2} {3} {4}", code, actorName, actorId,
                    Global.IdManager.GetHostName(hostId), hostAddr));
                
                ErrCode retCode = (code == DefaultErrCode.OK ? ErrCode.OK : ErrCode.ERROR);
                callback(
                    retCode,
                    actorName,
                    hostId,
                    Global.IdManager.GetHostName(hostId),
                    Global.IdManager.GetExtAddress(hostAddr)
                );

                LoginDb.Set(username, TimeUtil.GetTimeStampMS(), expireSec: 3600);
            });
        }

        [ServerApi]
        public void ResetPassword(string username, string email)
        {

        }
    }
}
