
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
                    
//using MessagePack;
using System;
using System.Threading.Tasks;

namespace Server
{

    [RefType("LoginService")]
    public partial class LoginServiceRef : ActorRef
    {
        public new bool isClient => false;
#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> rpc_create_account_async(dynamic username, dynamic password, dynamic callback=null)
#else
        public async Task<__ServerGModule__LoginService__CreateAccountReq.Callback> rpc_create_account_async(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__CreateAccountReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__CreateAccountReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new __ServerGModule__LoginService__CreateAccountReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, _cb });
            }
            else
            {
                Action<__ServerGModule__LoginService__CreateAccountReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __ServerGModule__LoginService__CreateAccountReq()
                    {
                         username=username,
                         password=password
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __ServerGModule__LoginService__CreateAccountReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__CreateAccountReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void rpc_create_account(dynamic username, dynamic password, dynamic callback)
#else
        public void rpc_create_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, callback });
                return;
            }
            Task.Run(() => {
                var msg = new __ServerGModule__LoginService__CreateAccountReq()
                {
                    username=username,
                    password=password
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __ServerGModule__LoginService__CreateAccountReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__CreateAccountReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__CREATE_ACCOUNT_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> rpc_delete_account_async(dynamic username, dynamic password, dynamic callback=null)
#else
        public async Task<__ServerGModule__LoginService__DeleteAccountReq.Callback> rpc_delete_account_async(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__DeleteAccountReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__DeleteAccountReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new __ServerGModule__LoginService__DeleteAccountReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, _cb });
            }
            else
            {
                Action<__ServerGModule__LoginService__DeleteAccountReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __ServerGModule__LoginService__DeleteAccountReq()
                    {
                         username=username,
                         password=password
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __ServerGModule__LoginService__DeleteAccountReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__DeleteAccountReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void rpc_delete_account(dynamic username, dynamic password, dynamic callback)
#else
        public void rpc_delete_account(global::System.String username, global::System.String password, global::System.Action<global::Shared.Protocol.ErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, callback });
                return;
            }
            Task.Run(() => {
                var msg = new __ServerGModule__LoginService__DeleteAccountReq()
                {
                    username=username,
                    password=password
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __ServerGModule__LoginService__DeleteAccountReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__DeleteAccountReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__DELETE_ACCOUNT_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> rpc_login_async(dynamic username, dynamic password, dynamic extraData, dynamic callback=null)
#else
        public async Task<__ServerGModule__LoginService__LoginReq.Callback> rpc_login_async(global::System.String username, global::System.String password, global::System.String extraData, global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__LoginReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__ServerGModule__LoginService__LoginReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String> _cb = (code, arg1, arg2, arg3, arg4) =>
                {
                     var cbMsg = new __ServerGModule__LoginService__LoginReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     cbMsg.arg2=arg2;
                     cbMsg.arg3=arg3;
                     cbMsg.arg4=arg4;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2, cbMsg.arg3, cbMsg.arg4);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, extraData, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, extraData, _cb });
            }
            else
            {
                Action<__ServerGModule__LoginService__LoginReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2, cbMsg.arg3, cbMsg.arg4);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __ServerGModule__LoginService__LoginReq()
                    {
                         username=username,
                         password=password,
                         extraData=extraData
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __ServerGModule__LoginService__LoginReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__LoginReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void rpc_login(dynamic username, dynamic password, dynamic extraData, dynamic callback)
#else
        public void rpc_login(global::System.String username, global::System.String password, global::System.String extraData, global::System.Action<global::Shared.Protocol.ErrCode, global::System.String, global::System.UInt64, global::System.String, global::System.String> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, password, extraData, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, password, extraData, callback });
                return;
            }
            Task.Run(() => {
                var msg = new __ServerGModule__LoginService__LoginReq()
                {
                    username=username,
                    password=password,
                    extraData=extraData
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __ServerGModule__LoginService__LoginReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerGModule__LoginService__LoginReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2, cbMsg.arg3, cbMsg.arg4);
                });
                this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ, msg, cb);
            });
#endif
        }


#if FENIX_CODEGEN && !RUNTIME
        public void rpc_reset_password(dynamic username, dynamic email)
#else
        public void rpc_reset_password(global::System.String username, global::System.String email)
#endif
        {
#if !FENIX_CODEGEN
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { username, email, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { username, email }); 
               return;
           }
           Task.Run(() => {
               var msg = new __ServerGModule__LoginService__ResetPasswordReq()
               {
                    username=username,
                    email=email
               };
               this.CallRemoteMethod(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ, msg, null);
            });
#endif
        }
    }
}

