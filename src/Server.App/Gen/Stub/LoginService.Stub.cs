
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
using Server.DataModel;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Server.GModule
{
    public partial class LoginService
    {
#if CLIENT


#endif
#if !CLIENT
        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerGModule__LoginService__create_account(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerGModule__LoginService__CreateAccountReq)msg;
            this.CreateAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new __ServerGModule__LoginService__CreateAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerGModule__LoginService__delete_account(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerGModule__LoginService__DeleteAccountReq)msg;
            this.DeleteAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new __ServerGModule__LoginService__DeleteAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerGModule__LoginService__login(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerGModule__LoginService__LoginReq)msg;
            this.Login(_msg.username, _msg.password, (code, arg1, arg2, arg3, arg4) =>
            {
                var cbMsg = new __ServerGModule__LoginService__LoginReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cbMsg.arg2=arg2;
                cbMsg.arg3=arg3;
                cbMsg.arg4=arg4;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerGModule__LoginService__reset_password(IMessage msg)
        {
            var _msg = (__ServerGModule__LoginService__ResetPasswordReq)msg;
            this.ResetPassword(_msg.username, _msg.email);
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__create_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            this.CreateAccount(username, password, callback);
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__delete_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            this.DeleteAccount(username, password, callback);
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__login(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String> callback)
        {
            this.Login(username, password, callback);
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__reset_password(global::System.String username, global::System.String email)
        {
            this.ResetPassword(username, email);
        }

#endif
    }
}

