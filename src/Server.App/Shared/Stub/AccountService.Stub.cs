
//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.Protocol;
using Shared.Protocol.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace GModule.Login
{
    public partial class AccountService
    {
        [RpcMethod(ProtocolCode.CREATE_ACCOUNT_REQ)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_SERVER_API_create_account(IMessage msg, Action<object> cb)
        {
            var _msg = (CreateAccountReq)msg;
            this.CreateAccount(_msg.username, _msg.password, _msg.extra, (code) =>
            {
                var cbMsg = new CreateAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.DELETE_ACCOUNT_REQ)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_SERVER_API_delete_account(IMessage msg, Action<object> cb)
        {
            var _msg = (DeleteAccountReq)msg;
            this.DeleteAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new DeleteAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.LOGIN_REQ)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_SERVER_API_login(IMessage msg, Action<object> cb)
        {
            var _msg = (LoginReq)msg;
            this.Login(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new LoginReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.RESET_PASSWORD_REQ)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_SERVER_API_reset_password(IMessage msg, Action<object> cb)
        {
            var _msg = (ResetPasswordReq)msg;
        }
    }
}

