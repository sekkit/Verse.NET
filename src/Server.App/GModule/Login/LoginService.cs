//LoginService

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Redis;
using Server.Config;
using Server.DataModel;
using Server.UModule;
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
        public async Task Login(string username, string password, string extraData, Action<ErrCode, string, ulong, string, string> callback)
        {
            Log.Info(string.Format("login_prepare {0} {1}", username, password));
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

            LoginDb.SetWithoutLock(username, (long)-1, expireSec:3);

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
                    Log.Info("kick_begin"); 
                    var result = await ActorRef().RemoveClientActorAsync(actorId, DisconnectReason.KICKED);
                    Log.Info("kick_end", result.code);
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

                LoginDb.SetWithoutLock(username, TimeUtil.GetTimeStampMS(), expireSec: 3600);
                return;
            }

            Log.Info("login_create_actor", account.uid);

            //如果不存在，则申请创建一个
            var svc = GetService<MasterServiceRef>();
            svc.CreateActor(nameof(Avatar), account.uid, (code, actorInfo) =>
            {
                Global.IdManager.RegisterActorInfo(actorInfo);
                actorId = actorInfo.ActorId;
                Log.Info("create_actor:", code, actorInfo.ActorName, actorInfo.ActorId);
                if (code != DefaultErrCode.OK && code != DefaultErrCode.create_actor_already_exists)
                {
                    Log.Error("create_actor_fail", code);
                    callback(ErrCode.ERROR, actorInfo.ActorName, 0, null, null);
                    LoginDb.Delete(username);
                    return;
                }

                var hostId = Global.IdManager.GetHostIdByActorId(actorInfo.ActorId); //, false);
                //创建成功后，把客户端的avatar注册到服务端
                var hostAddr = Global.IdManager.GetHostAddrByActorId(actorInfo.ActorId);
                Log.Info(string.Format("login.create_actor@Master.App {0} {1} {2} {3} {4}", code, actorInfo.ActorName, actorInfo.ActorId,
                    Global.IdManager.GetHostName(hostId), hostAddr));
                
                var retCode = (code == DefaultErrCode.OK ? ErrCode.OK : ErrCode.ERROR);
                callback(
                    retCode,
                    actorInfo.ActorName,
                    hostId,
                    Global.IdManager.GetHostName(hostId),
                    Global.IdManager.GetExtAddress(hostAddr)
                );

                LoginDb.SetWithoutLock(username, TimeUtil.GetTimeStampMS(), expireSec: 3600);
            });
        }

        [ServerApi]
        public void ResetPassword(string username, string email)
        {

        }
    }
}
