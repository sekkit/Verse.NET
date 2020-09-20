#if !CLIENT

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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
        public async Task<dynamic> CreateActorAsync(dynamic typename, dynamic name, dynamic callback=null)
#else
        public async Task<CreateActorReq.Callback> CreateActorAsync(global::System.String typename, global::System.String name, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.String, global::System.UInt64> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<CreateActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<CreateActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.String, global::System.UInt64> _cb = (code, arg1, arg2) =>
                {
                     var cbMsg = new CreateActorReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     cbMsg.arg2=arg2;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { typename, name, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { typename, name, _cb });
            }
            else
            {
                Action<CreateActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new CreateActorReq()
                    {
                         typename=typename,
                         name=name
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new CreateActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void CreateActor(dynamic typename, dynamic name, dynamic callback)
#else
        public void CreateActor(global::System.String typename, global::System.String name, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.String, global::System.UInt64> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { typename, name, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { typename, name, callback });
                return;
            }
            Task.Run(() => {
                var msg = new CreateActorReq()
                {
                    typename=typename,
                    name=name
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new CreateActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
                });
                this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> FindActorAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<FindActorReq.Callback> FindActorAsync(global::System.UInt64 actorId, global::System.Action<global::System.Boolean, global::Fenix.ActorInfo> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<FindActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<FindActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean, global::Fenix.ActorInfo> _cb = (arg0, arg1) =>
                {
                     var cbMsg = new FindActorReq.Callback();
                     cbMsg.arg0=arg0;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.FIND_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, _cb });
            }
            else
            {
                Action<FindActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new FindActorReq()
                    {
                         actorId=actorId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new FindActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<FindActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.FIND_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void FindActor(dynamic actorId, dynamic callback)
#else
        public void FindActor(global::System.UInt64 actorId, global::System.Action<global::System.Boolean, global::Fenix.ActorInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.FIND_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new FindActorReq()
                {
                    actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new FindActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<FindActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.FIND_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> FindHostAsync(dynamic hostId, dynamic callback=null)
#else
        public async Task<FindHostReq.Callback> FindHostAsync(global::System.UInt64 hostId, global::System.Action<global::System.Boolean, global::Fenix.HostInfo> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<FindHostReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<FindHostReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean, global::Fenix.HostInfo> _cb = (arg0, arg1) =>
                {
                     var cbMsg = new FindHostReq.Callback();
                     cbMsg.arg0=arg0;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.FIND_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, _cb });
            }
            else
            {
                Action<FindHostReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new FindHostReq()
                    {
                         hostId=hostId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new FindHostReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<FindHostReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.FIND_HOST_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void FindHost(dynamic hostId, dynamic callback)
#else
        public void FindHost(global::System.UInt64 hostId, global::System.Action<global::System.Boolean, global::Fenix.HostInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.FIND_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new FindHostReq()
                {
                    hostId=hostId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new FindHostReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<FindHostReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.FIND_HOST_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> MigrateActorAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<MigrateActorReq.Callback> MigrateActorAsync(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[]> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<MigrateActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<MigrateActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[]> _cb = (code, arg1) =>
                {
                     var cbMsg = new MigrateActorReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, _cb });
            }
            else
            {
                Action<MigrateActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new MigrateActorReq()
                    {
                         actorId=actorId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new MigrateActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<MigrateActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void MigrateActor(dynamic actorId, dynamic callback)
#else
        public void MigrateActor(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[]> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new MigrateActorReq()
                {
                    actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new MigrateActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<MigrateActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnBeforeDisconnectAsync(dynamic reason, dynamic callback=null)
#else
        public async Task<OnBeforeDisconnectNtf.Callback> OnBeforeDisconnectAsync(global::Fenix.Common.DisconnectReason reason, global::System.Action callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnBeforeDisconnectNtf.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnBeforeDisconnectNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action _cb = () =>
                {
                     var cbMsg = new OnBeforeDisconnectNtf.Callback();

                     callback?.Invoke();
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_BEFORE_DISCONNECT_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { reason, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { reason, _cb });
            }
            else
            {
                Action<OnBeforeDisconnectNtf.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke();
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnBeforeDisconnectNtf()
                    {
                         reason=reason
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnBeforeDisconnectNtf.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnBeforeDisconnectNtf.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_BEFORE_DISCONNECT_NTF, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnBeforeDisconnect(dynamic reason, dynamic callback)
#else
        public void OnBeforeDisconnect(global::Fenix.Common.DisconnectReason reason, global::System.Action callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_BEFORE_DISCONNECT_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { reason, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { reason, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnBeforeDisconnectNtf()
                {
                    reason=reason
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnBeforeDisconnectNtf.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnBeforeDisconnectNtf.Callback>(cbData);
                    callback?.Invoke();
                });
                this.CallRemoteMethod(OpCode.ON_BEFORE_DISCONNECT_NTF, msg, cb);
            });
#endif
        }


#if FENIX_CODEGEN && !RUNTIME
        public void OnServerActorEnable(dynamic actorName)
#else
        public void OnServerActorEnable(global::System.String actorName)
#endif
        {
#if !FENIX_CODEGEN
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = OpCode.ON_SERVER_ACTOR_ENABLE_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorName, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorName }); 
               return;
           }
           Task.Run(() => {
               var msg = new OnServerActorEnableNtf()
               {
                    actorName=actorName
               };
               this.CallRemoteMethod(OpCode.ON_SERVER_ACTOR_ENABLE_NTF, msg, null);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> ReconnectServerActorAsync(dynamic hostId, dynamic hostName, dynamic hostIP, dynamic hostPort, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback=null)
#else
        public async Task<ReconnectServerActorNtf.Callback> ReconnectServerActorAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.String hostIP, global::System.Int32 hostPort, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::Fenix.Common.DefaultErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<ReconnectServerActorNtf.Callback>();
#endif
#else
            var t = new TaskCompletionSource<ReconnectServerActorNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new ReconnectServerActorNtf.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.RECONNECT_SERVER_ACTOR_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, _cb });
            }
            else
            {
                Action<ReconnectServerActorNtf.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new ReconnectServerActorNtf()
                    {
                         hostId=hostId,
                         hostName=hostName,
                         hostIP=hostIP,
                         hostPort=hostPort,
                         actorId=actorId,
                         actorName=actorName,
                         aTypeName=aTypeName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new ReconnectServerActorNtf.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<ReconnectServerActorNtf.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void ReconnectServerActor(dynamic hostId, dynamic hostName, dynamic hostIP, dynamic hostPort, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback)
#else
        public void ReconnectServerActor(global::System.UInt64 hostId, global::System.String hostName, global::System.String hostIP, global::System.Int32 hostPort, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::Fenix.Common.DefaultErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.RECONNECT_SERVER_ACTOR_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, callback });
                return;
            }
            Task.Run(() => {
                var msg = new ReconnectServerActorNtf()
                {
                    hostId=hostId,
                    hostName=hostName,
                    hostIP=hostIP,
                    hostPort=hostPort,
                    actorId=actorId,
                    actorName=actorName,
                    aTypeName=aTypeName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new ReconnectServerActorNtf.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<ReconnectServerActorNtf.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RegisterAsync(dynamic hostId, dynamic hostName, dynamic callback=null)
#else
        public async Task<RegisterReq.Callback> RegisterAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RegisterReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RegisterReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> _cb = (code, arg1) =>
                {
                     var cbMsg = new RegisterReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REGISTER_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                Action<RegisterReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RegisterReq()
                    {
                         hostId=hostId,
                         hostName=hostName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RegisterReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REGISTER_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void Register(dynamic hostId, dynamic hostName, dynamic callback)
#else
        public void Register(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.HostInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new RegisterReq()
                {
                    hostId=hostId,
                    hostName=hostName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RegisterReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.REGISTER_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RegisterActorAsync(dynamic hostId, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback=null)
#else
        public async Task<RegisterActorReq.Callback> RegisterActorAsync(global::System.UInt64 hostId, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RegisterActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RegisterActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RegisterActorReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REGISTER_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, actorId, actorName, aTypeName, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, actorId, actorName, aTypeName, _cb });
            }
            else
            {
                Action<RegisterActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RegisterActorReq()
                    {
                         hostId=hostId,
                         actorId=actorId,
                         actorName=actorName,
                         aTypeName=aTypeName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RegisterActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REGISTER_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RegisterActor(dynamic hostId, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback)
#else
        public void RegisterActor(global::System.UInt64 hostId, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, actorId, actorName, aTypeName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, actorId, actorName, aTypeName, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RegisterActorReq()
                {
                    hostId=hostId,
                    actorId=actorId,
                    actorName=actorName,
                    aTypeName=aTypeName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RegisterActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REGISTER_ACTOR_REQ, msg, cb);
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
        public async Task<dynamic> RegisterHostAsync(dynamic hostId, dynamic hostName, dynamic intAddr, dynamic extAddr, dynamic callback=null)
#else
        public async Task<RegisterHostReq.Callback> RegisterHostAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.String intAddr, global::System.String extAddr, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RegisterHostReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RegisterHostReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RegisterHostReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REGISTER_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, intAddr, extAddr, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, intAddr, extAddr, _cb });
            }
            else
            {
                Action<RegisterHostReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RegisterHostReq()
                    {
                         hostId=hostId,
                         hostName=hostName,
                         intAddr=intAddr,
                         extAddr=extAddr
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RegisterHostReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterHostReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REGISTER_HOST_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RegisterHost(dynamic hostId, dynamic hostName, dynamic intAddr, dynamic extAddr, dynamic callback)
#else
        public void RegisterHost(global::System.UInt64 hostId, global::System.String hostName, global::System.String intAddr, global::System.String extAddr, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, intAddr, extAddr, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, intAddr, extAddr, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RegisterHostReq()
                {
                    hostId=hostId,
                    hostName=hostName,
                    intAddr=intAddr,
                    extAddr=extAddr
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RegisterHostReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RegisterHostReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REGISTER_HOST_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RemoveActorAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<RemoveActorReq.Callback> RemoveActorAsync(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveActorReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RemoveActorReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, _cb });
            }
            else
            {
                Action<RemoveActorReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveActorReq()
                    {
                         actorId=actorId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveActorReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveActorReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveActor(dynamic actorId, dynamic callback)
#else
        public void RemoveActor(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RemoveActorReq()
                {
                    actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveActorReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveActorReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, cb);
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
        public async Task<dynamic> RemoveHostAsync(dynamic hostId, dynamic hostName, dynamic callback=null)
#else
        public async Task<RemoveHostReq.Callback> RemoveHostAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveHostReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveHostReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RemoveHostReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                Action<RemoveHostReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveHostReq()
                    {
                         hostId=hostId,
                         hostName=hostName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveHostReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveHostReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_HOST_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveHost(dynamic hostId, dynamic hostName, dynamic callback)
#else
        public void RemoveHost(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_HOST_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new RemoveHostReq()
                {
                    hostId=hostId,
                    hostName=hostName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveHostReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveHostReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REMOVE_HOST_REQ, msg, cb);
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
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

