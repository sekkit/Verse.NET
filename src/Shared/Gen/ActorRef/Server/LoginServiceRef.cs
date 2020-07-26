
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;

using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;

using MessagePack; 
using System;

namespace Server
{

    [RefType("LoginService")]
    public partial class LoginServiceRef : ActorRef
    {
        public void rpc_create_account(String username, String password, String extra, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.CREATE_ACCOUNT_REQ, new object[] { username, password, extra, callback });
                return;
            }
            var msg = new CreateAccountReq()
            {
                username=username,
                password=password,
                extra=extra
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new CreateAccountReq.Callback():RpcUtil.Deserialize<CreateAccountReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.CREATE_ACCOUNT_REQ, msg, cb);
        }

        public void rpc_delete_account(String username, String password, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.DELETE_ACCOUNT_REQ, new object[] { username, password, callback });
                return;
            }
            var msg = new DeleteAccountReq()
            {
                username=username,
                password=password
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new DeleteAccountReq.Callback():RpcUtil.Deserialize<DeleteAccountReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.DELETE_ACCOUNT_REQ, msg, cb);
        }

        public void rpc_login(String username, String password, Action<ErrCode, String, UInt32, String, String> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.LOGIN_REQ, new object[] { username, password, callback });
                return;
            }
            var msg = new LoginReq()
            {
                username=username,
                password=password
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new LoginReq.Callback():RpcUtil.Deserialize<LoginReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2, cbMsg.arg3, cbMsg.arg4);
            });
            this.CallRemoteMethod(ProtocolCode.LOGIN_REQ, msg, cb);
        }

        public void rpc_reset_password(String username, String email)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.RESET_PASSWORD_REQ, new object[] { username, email });
               return;
           }
           var msg = new ResetPasswordReq()
           {
                username=username,
                email=email
           };
           this.CallRemoteMethod(ProtocolCode.RESET_PASSWORD_REQ, msg, null);
        }
    }
}

