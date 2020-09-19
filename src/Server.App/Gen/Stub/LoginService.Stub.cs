
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
#if !FENIX_CODEGEN
#if CLIENT


#endif
#if !CLIENT
        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, Api.ServerApi)]
        public void SERVER_API__ServerGModule__LoginService__create_account(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerGModule__LoginService__CreateAccountReq)msg;
            this.CreateAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new __ServerGModule__LoginService__CreateAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.CreateAccount(_msg.username, _msg.password, (global::System.Action<global::Shared.Protocol.ErrCode>)((code) =>
            {
                dynamic cbMsg = new __ServerGModule__LoginService__CreateAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, Api.ServerApi)]
        public void SERVER_API__ServerGModule__LoginService__delete_account(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerGModule__LoginService__DeleteAccountReq)msg;
            this.DeleteAccount(_msg.username, _msg.password, (code) =>
            {
                var cbMsg = new __ServerGModule__LoginService__DeleteAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.DeleteAccount(_msg.username, _msg.password, (global::System.Action<global::Shared.Protocol.ErrCode>)((code) =>
            {
                dynamic cbMsg = new __ServerGModule__LoginService__DeleteAccountReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, Api.ServerApi)]
        public void SERVER_API__ServerGModule__LoginService__login(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerGModule__LoginService__LoginReq)msg;
            this.Login(_msg.username, _msg.password, _msg.extraData, (code, arg1, arg2, arg3, arg4) =>
            {
                var cbMsg = new __ServerGModule__LoginService__LoginReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cbMsg.arg2=arg2;
                cbMsg.arg3=arg3;
                cbMsg.arg4=arg4;
                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.Login(_msg.username, _msg.password, _msg.extraData, (global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String>)((code, arg1, arg2, arg3, arg4) =>
            {
                dynamic cbMsg = new __ServerGModule__LoginService__LoginReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cbMsg.arg2=arg2;
                cbMsg.arg3=arg3;
                cbMsg.arg4=arg4;
                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ, Api.ServerApi)]
        public void SERVER_API__ServerGModule__LoginService__reset_password(IMessage msg)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerGModule__LoginService__ResetPasswordReq)msg;
            this.ResetPassword(_msg.username, _msg.email);
#else
            dynamic _msg = msg;
            self.ResetPassword(_msg.username, _msg.email);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__create_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.CreateAccount(username, password, callback);
#else
            self.CreateAccount(username, password, callback);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__delete_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.DeleteAccount(username, password, callback);
#else
            self.DeleteAccount(username, password, callback);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__login(global::System.String username, global::System.String password, global::System.String extraData, global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String> callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.Login(username, password, extraData, callback);
#else
            self.Login(username, password, extraData, callback);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerGModule__LoginService__reset_password(global::System.String username, global::System.String email)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.ResetPassword(username, email);
#else
            self.ResetPassword(username, email);
#endif
        }

#endif
#endif
    }
}

