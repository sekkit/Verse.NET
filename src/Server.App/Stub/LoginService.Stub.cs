
//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Server.GModule
{
    public partial class LoginService
    {
        [RpcMethod(ProtocolCode.CREATE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_create_account(IMessage msg, Action<object> cb)
        {
            var _msg = (CreateAccountReq)msg;
            this.CreateAccount(_msg.username, _msg.password, _msg.extra, (code) =>
            {
                var cbMsg = new CreateAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.DELETE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_delete_account(IMessage msg, Action<object> cb)
        {
            var _msg = (DeleteAccountReq)msg;
            this.DeleteAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new DeleteAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.LOGIN_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_login(IMessage msg, Action<object> cb)
        {
            var _msg = (LoginReq)msg;
            this.Login(_msg.username, _msg.password, (code, arg1, arg2, arg3, arg4) =>
            {
                var cbMsg = new LoginReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cbMsg.arg2=arg2;
                cbMsg.arg3=arg3;
                cbMsg.arg4=arg4;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.RESET_PASSWORD_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_reset_password(IMessage msg)
        {
            var _msg = (ResetPasswordReq)msg;
            this.ResetPassword(_msg.username, _msg.email);
        }
    }
}

