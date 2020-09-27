#if CLIENT

//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;


//using MessagePack;
using System;
using System.Threading.Tasks;

namespace Fenix
{

    public partial class ActorRef
    {
#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> BindClientActorAsync(dynamic actorName, dynamic callback=null)
#else
        public async Task<BindClientActorReq.Callback> BindClientActorAsync(global::System.String actorName, global::System.Action<global::Fenix.Common.DefaultErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<BindClientActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<BindClientActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new BindClientActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.BIND_CLIENT_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorName, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorName, _cb });
            }
            else
            {
                Action<BindClientActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new BindClientActorReq()
                    {
                         actorName=actorName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new BindClientActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void BindClientActor(dynamic actorName, dynamic callback)
#else
        public void BindClientActor(global::System.String actorName, global::System.Action<global::Fenix.Common.DefaultErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.BIND_CLIENT_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorName, callback });
                return;
            }
            Task.Run(() => {
                var msg = new BindClientActorReq()
                {
                    actorName=actorName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new BindClientActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RegisterClientAsync(dynamic hostId, dynamic hostName, dynamic callback=null)
#else
        public async Task<RegisterClientReq.Callback> RegisterClientAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RegisterClientReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RegisterClientReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> _cb = (code, arg1) =>
                {
                     var cbMsg = new RegisterClientReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REGISTER_CLIENT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, _cb });
            }
            else
            {
                Action<RegisterClientReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RegisterClientReq()
                    {
                         hostId=hostId,
                         hostName=hostName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RegisterClientReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RegisterClient(dynamic hostId, dynamic hostName, dynamic callback)
#else
        public void RegisterClient(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_CLIENT_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RegisterClientReq()
                {
                    hostId=hostId,
                    hostName=hostName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RegisterClientReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RemoveClientActorAsync(dynamic actorId, dynamic reason, dynamic callback=null)
#else
        public async Task<RemoveClientActorReq.Callback> RemoveClientActorAsync(global::System.UInt64 actorId, global::Fenix.Common.DisconnectReason reason, global::System.Action<global::Fenix.Common.DefaultErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveClientActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveClientActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new RemoveClientActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_CLIENT_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, reason, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, reason, _cb });
            }
            else
            {
                Action<RemoveClientActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveClientActorReq()
                    {
                         actorId=actorId,
                         reason=reason
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveClientActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveClientActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_CLIENT_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveClientActor(dynamic actorId, dynamic reason, dynamic callback)
#else
        public void RemoveClientActor(global::System.UInt64 actorId, global::Fenix.Common.DisconnectReason reason, global::System.Action<global::Fenix.Common.DefaultErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_CLIENT_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, reason, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, reason, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RemoveClientActorReq()
                {
                    actorId=actorId,
                    reason=reason
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveClientActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveClientActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(OpCode.REMOVE_CLIENT_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> SayHelloAsync(dynamic callback=null)
#else
        public async Task<SayHelloReq.Callback> SayHelloAsync(global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<SayHelloReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<SayHelloReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> _cb = (code, arg1) =>
                {
                     var cbMsg = new SayHelloReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.SAY_HELLO_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { _cb });
            }
            else
            {
                Action<SayHelloReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new SayHelloReq()
                    {

                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new SayHelloReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<SayHelloReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.SAY_HELLO_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void SayHello(dynamic callback)
#else
        public void SayHello(global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.SAY_HELLO_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { callback });
                return;
            }
            Task.Run(() => {
                var msg = new SayHelloReq()
                {

                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new SayHelloReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<SayHelloReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.SAY_HELLO_REQ, msg, cb);
            });
#endif
        }
    }
}
#endif

