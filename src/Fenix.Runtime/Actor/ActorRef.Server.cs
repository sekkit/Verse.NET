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
        public async Task<dynamic> AddActorIdAsync(dynamic hostId, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback=null)
#else
        public async Task<AddActorIdReq.Callback> AddActorIdAsync(global::System.UInt64 hostId, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<AddActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<AddActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new AddActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ADD_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<AddActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new AddActorIdReq()
                    {
                         hostId=hostId,
                         actorId=actorId,
                         actorName=actorName,
                         aTypeName=aTypeName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new AddActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<AddActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ADD_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void AddActorId(dynamic hostId, dynamic actorId, dynamic actorName, dynamic aTypeName, dynamic callback)
#else
        public void AddActorId(global::System.UInt64 hostId, global::System.UInt64 actorId, global::System.String actorName, global::System.String aTypeName, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ADD_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new AddActorIdReq()
                {
                    hostId=hostId,
                    actorId=actorId,
                    actorName=actorName,
                    aTypeName=aTypeName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new AddActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<AddActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ADD_ACTOR_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> AddClientActorIdAsync(dynamic fromHostId, dynamic clientId, dynamic actorId, dynamic actorName, dynamic address, dynamic callback=null)
#else
        public async Task<AddClientActorIdReq.Callback> AddClientActorIdAsync(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.UInt64 actorId, global::System.String actorName, global::System.String address, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<AddClientActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<AddClientActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new AddClientActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ADD_CLIENT_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, actorId, actorName, address, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, actorId, actorName, address, _cb });
            }
            else
            {
                Action<AddClientActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new AddClientActorIdReq()
                    {
                         fromHostId=fromHostId,
                         clientId=clientId,
                         actorId=actorId,
                         actorName=actorName,
                         address=address
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new AddClientActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<AddClientActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ADD_CLIENT_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void AddClientActorId(dynamic fromHostId, dynamic clientId, dynamic actorId, dynamic actorName, dynamic address, dynamic callback)
#else
        public void AddClientActorId(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.UInt64 actorId, global::System.String actorName, global::System.String address, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ADD_CLIENT_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, actorId, actorName, address, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, actorId, actorName, address, callback });
                return;
            }
            Task.Run(() => {
                var msg = new AddClientActorIdReq()
                {
                    fromHostId=fromHostId,
                    clientId=clientId,
                    actorId=actorId,
                    actorName=actorName,
                    address=address
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new AddClientActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<AddClientActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ADD_CLIENT_ACTOR_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> AddClientHostIdAsync(dynamic fromHostId, dynamic clientId, dynamic clientName, dynamic extAddr, dynamic callback=null)
#else
        public async Task<AddClientHostIdReq.Callback> AddClientHostIdAsync(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.String clientName, global::System.String extAddr, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<AddClientHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<AddClientHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new AddClientHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ADD_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, clientName, extAddr, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, clientName, extAddr, _cb });
            }
            else
            {
                Action<AddClientHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new AddClientHostIdReq()
                    {
                         fromHostId=fromHostId,
                         clientId=clientId,
                         clientName=clientName,
                         extAddr=extAddr
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new AddClientHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<AddClientHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ADD_CLIENT_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void AddClientHostId(dynamic fromHostId, dynamic clientId, dynamic clientName, dynamic extAddr, dynamic callback)
#else
        public void AddClientHostId(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.String clientName, global::System.String extAddr, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ADD_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, clientName, extAddr, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, clientName, extAddr, callback });
                return;
            }
            Task.Run(() => {
                var msg = new AddClientHostIdReq()
                {
                    fromHostId=fromHostId,
                    clientId=clientId,
                    clientName=clientName,
                    extAddr=extAddr
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new AddClientHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<AddClientHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ADD_CLIENT_HOST_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> AddHostIdAsync(dynamic hostId, dynamic hostName, dynamic intAddr, dynamic extAddr, dynamic callback=null)
#else
        public async Task<AddHostIdReq.Callback> AddHostIdAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.String intAddr, global::System.String extAddr, global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<AddHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<AddHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> _cb = (arg0, arg1) =>
                {
                     var cbMsg = new AddHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ADD_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<AddHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new AddHostIdReq()
                    {
                         hostId=hostId,
                         hostName=hostName,
                         intAddr=intAddr,
                         extAddr=extAddr
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new AddHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<AddHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ADD_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void AddHostId(dynamic hostId, dynamic hostName, dynamic intAddr, dynamic extAddr, dynamic callback)
#else
        public void AddHostId(global::System.UInt64 hostId, global::System.String hostName, global::System.String intAddr, global::System.String extAddr, global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ADD_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new AddHostIdReq()
                {
                    hostId=hostId,
                    hostName=hostName,
                    intAddr=intAddr,
                    extAddr=extAddr
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new AddHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<AddHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.ADD_HOST_ID_REQ, msg, cb);
            });
#endif
        }

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
        public async Task<dynamic> CreateActorAsync(dynamic typename, dynamic name, dynamic callback=null)
#else
        public async Task<CreateActorReq.Callback> CreateActorAsync(global::System.String typename, global::System.String name, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.ActorInfo> callback=null)
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
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.ActorInfo> _cb = (code, arg1) =>
                {
                     var cbMsg = new CreateActorReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
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
        public void CreateActor(global::System.String typename, global::System.String name, global::System.Action<global::Fenix.Common.DefaultErrCode, global::Fenix.ActorInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> GetIdAllAsync(dynamic hostId, dynamic callback=null)
#else
        public async Task<GetIdAllReq.Callback> GetIdAllAsync(global::System.UInt64 hostId, global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<GetIdAllReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<GetIdAllReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> _cb = (arg0, arg1) =>
                {
                     var cbMsg = new GetIdAllReq.Callback();
                     cbMsg.arg0=arg0;
                     cbMsg.arg1=arg1;
                     callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.GET_ID_ALL_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<GetIdAllReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new GetIdAllReq()
                    {
                         hostId=hostId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new GetIdAllReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<GetIdAllReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.GET_ID_ALL_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void GetIdAll(dynamic hostId, dynamic callback)
#else
        public void GetIdAll(global::System.UInt64 hostId, global::System.Action<global::System.Boolean, global::Fenix.IdDataSet> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.GET_ID_ALL_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new GetIdAllReq()
                {
                    hostId=hostId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new GetIdAllReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<GetIdAllReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
                });
                this.CallRemoteMethod(OpCode.GET_ID_ALL_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> MigrateActorAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<MigrateActorReq.Callback> MigrateActorAsync(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[], global::Fenix.ActorInfo> callback=null)
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
                global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[], global::Fenix.ActorInfo> _cb = (code, arg1, arg2) =>
                {
                     var cbMsg = new MigrateActorReq.Callback();
                     cbMsg.code=code;
                     cbMsg.arg1=arg1;
                     cbMsg.arg2=arg2;
                     callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
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
        public void MigrateActor(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode, global::System.Byte[], global::Fenix.ActorInfo> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
                });
                this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnAddActorIdAsync(dynamic actorInfo, dynamic callback=null)
#else
        public async Task<OnAddActorIdReq.Callback> OnAddActorIdAsync(global::Fenix.ActorInfo actorInfo, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnAddActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnAddActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnAddActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_ADD_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorInfo, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorInfo, _cb });
            }
            else
            {
                Action<OnAddActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnAddActorIdReq()
                    {
                         actorInfo=actorInfo
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnAddActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_ADD_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnAddActorId(dynamic actorInfo, dynamic callback)
#else
        public void OnAddActorId(global::Fenix.ActorInfo actorInfo, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_ADD_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorInfo, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorInfo, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnAddActorIdReq()
                {
                    actorInfo=actorInfo
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnAddActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_ADD_ACTOR_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnAddClientActorIdAsync(dynamic clientId, dynamic actorId, dynamic actorName, dynamic address, dynamic callback=null)
#else
        public async Task<OnAddClientActorIdReq.Callback> OnAddClientActorIdAsync(global::System.UInt64 clientId, global::System.UInt64 actorId, global::System.String actorName, global::System.String address, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnAddClientActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnAddClientActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnAddClientActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_ADD_CLIENT_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, actorId, actorName, address, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, actorId, actorName, address, _cb });
            }
            else
            {
                Action<OnAddClientActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnAddClientActorIdReq()
                    {
                         clientId=clientId,
                         actorId=actorId,
                         actorName=actorName,
                         address=address
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnAddClientActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddClientActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_ADD_CLIENT_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnAddClientActorId(dynamic clientId, dynamic actorId, dynamic actorName, dynamic address, dynamic callback)
#else
        public void OnAddClientActorId(global::System.UInt64 clientId, global::System.UInt64 actorId, global::System.String actorName, global::System.String address, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_ADD_CLIENT_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, actorId, actorName, address, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, actorId, actorName, address, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnAddClientActorIdReq()
                {
                    clientId=clientId,
                    actorId=actorId,
                    actorName=actorName,
                    address=address
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnAddClientActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddClientActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_ADD_CLIENT_ACTOR_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnAddClientHostIdAsync(dynamic clientId, dynamic clientName, dynamic address, dynamic callback=null)
#else
        public async Task<OnAddClientHostIdReq.Callback> OnAddClientHostIdAsync(global::System.UInt64 clientId, global::System.String clientName, global::System.String address, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnAddClientHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnAddClientHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnAddClientHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_ADD_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, clientName, address, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, clientName, address, _cb });
            }
            else
            {
                Action<OnAddClientHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnAddClientHostIdReq()
                    {
                         clientId=clientId,
                         clientName=clientName,
                         address=address
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnAddClientHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddClientHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_ADD_CLIENT_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnAddClientHostId(dynamic clientId, dynamic clientName, dynamic address, dynamic callback)
#else
        public void OnAddClientHostId(global::System.UInt64 clientId, global::System.String clientName, global::System.String address, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_ADD_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, clientName, address, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, clientName, address, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnAddClientHostIdReq()
                {
                    clientId=clientId,
                    clientName=clientName,
                    address=address
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnAddClientHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddClientHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_ADD_CLIENT_HOST_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnAddHostIdAsync(dynamic hostInfo, dynamic callback=null)
#else
        public async Task<OnAddHostIdReq.Callback> OnAddHostIdAsync(global::Fenix.HostInfo hostInfo, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnAddHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnAddHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnAddHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_ADD_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostInfo, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostInfo, _cb });
            }
            else
            {
                Action<OnAddHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnAddHostIdReq()
                    {
                         hostInfo=hostInfo
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnAddHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_ADD_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnAddHostId(dynamic hostInfo, dynamic callback)
#else
        public void OnAddHostId(global::Fenix.HostInfo hostInfo, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_ADD_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostInfo, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostInfo, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnAddHostIdReq()
                {
                    hostInfo=hostInfo
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnAddHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnAddHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_ADD_HOST_ID_REQ, msg, cb);
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
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
        public async Task<dynamic> OnRemoveActorIdAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<OnRemoveActorIdReq.Callback> OnRemoveActorIdAsync(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnRemoveActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnRemoveActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnRemoveActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_REMOVE_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<OnRemoveActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnRemoveActorIdReq()
                    {
                         actorId=actorId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnRemoveActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_REMOVE_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnRemoveActorId(dynamic actorId, dynamic callback)
#else
        public void OnRemoveActorId(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_REMOVE_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new OnRemoveActorIdReq()
                {
                    actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnRemoveActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_REMOVE_ACTOR_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnRemoveClientHostIdAsync(dynamic clientId, dynamic callback=null)
#else
        public async Task<OnRemoveClientHostIdReq.Callback> OnRemoveClientHostIdAsync(global::System.UInt64 clientId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnRemoveClientHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnRemoveClientHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnRemoveClientHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_REMOVE_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, _cb });
            }
            else
            {
                Action<OnRemoveClientHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnRemoveClientHostIdReq()
                    {
                         clientId=clientId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnRemoveClientHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveClientHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_REMOVE_CLIENT_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnRemoveClientHostId(dynamic clientId, dynamic callback)
#else
        public void OnRemoveClientHostId(global::System.UInt64 clientId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_REMOVE_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { clientId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { clientId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new OnRemoveClientHostIdReq()
                {
                    clientId=clientId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnRemoveClientHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveClientHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_REMOVE_CLIENT_HOST_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> OnRemoveHostIdAsync(dynamic hostId, dynamic callback=null)
#else
        public async Task<OnRemoveHostIdReq.Callback> OnRemoveHostIdAsync(global::System.UInt64 hostId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<OnRemoveHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<OnRemoveHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new OnRemoveHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_REMOVE_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<OnRemoveHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new OnRemoveHostIdReq()
                    {
                         hostId=hostId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new OnRemoveHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.ON_REMOVE_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void OnRemoveHostId(dynamic hostId, dynamic callback)
#else
        public void OnRemoveHostId(global::System.UInt64 hostId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_REMOVE_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new OnRemoveHostIdReq()
                {
                    hostId=hostId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new OnRemoveHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<OnRemoveHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.ON_REMOVE_HOST_ID_REQ, msg, cb);
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
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
        public async Task<dynamic> RemoveActorAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<RemoveActorReq.Callback> RemoveActorAsync(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode> callback=null)
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
                global::System.Action<global::Fenix.Common.DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new RemoveActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code);
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
        public void RemoveActor(global::System.UInt64 actorId, global::System.Action<global::Fenix.Common.DefaultErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RemoveActorIdAsync(dynamic actorId, dynamic callback=null)
#else
        public async Task<RemoveActorIdReq.Callback> RemoveActorIdAsync(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveActorIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveActorIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RemoveActorIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                Action<RemoveActorIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveActorIdReq()
                    {
                         actorId=actorId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveActorIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveActorIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_ACTOR_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveActorId(dynamic actorId, dynamic callback)
#else
        public void RemoveActorId(global::System.UInt64 actorId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_ACTOR_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
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
                var msg = new RemoveActorIdReq()
                {
                    actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveActorIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveActorIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REMOVE_ACTOR_ID_REQ, msg, cb);
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
        public async Task<dynamic> RemoveClientHostIdAsync(dynamic fromHostId, dynamic clientId, dynamic callback=null)
#else
        public async Task<RemoveClientHostIdReq.Callback> RemoveClientHostIdAsync(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveClientHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveClientHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RemoveClientHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, _cb });
            }
            else
            {
                Action<RemoveClientHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveClientHostIdReq()
                    {
                         fromHostId=fromHostId,
                         clientId=clientId
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveClientHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveClientHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_CLIENT_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveClientHostId(dynamic fromHostId, dynamic clientId, dynamic callback)
#else
        public void RemoveClientHostId(global::System.UInt64 fromHostId, global::System.UInt64 clientId, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_CLIENT_HOST_ID_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { fromHostId, clientId, callback });
                return;
            }
            Task.Run(() => {
                var msg = new RemoveClientHostIdReq()
                {
                    fromHostId=fromHostId,
                    clientId=clientId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveClientHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveClientHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REMOVE_CLIENT_HOST_ID_REQ, msg, cb);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> RemoveHostIdAsync(dynamic hostId, dynamic hostName, dynamic callback=null)
#else
        public async Task<RemoveHostIdReq.Callback> RemoveHostIdAsync(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::System.Boolean> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<RemoveHostIdReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<RemoveHostIdReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::System.Boolean> _cb = (arg0) =>
                {
                     var cbMsg = new RemoveHostIdReq.Callback();
                     cbMsg.arg0=arg0;
                     callback?.Invoke(cbMsg.arg0);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_HOST_ID_REQ;
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
                Action<RemoveHostIdReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.arg0);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new RemoveHostIdReq()
                    {
                         hostId=hostId,
                         hostName=hostName
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new RemoveHostIdReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveHostIdReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(OpCode.REMOVE_HOST_ID_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void RemoveHostId(dynamic hostId, dynamic hostName, dynamic callback)
#else
        public void RemoveHostId(global::System.UInt64 hostId, global::System.String hostName, global::System.Action<global::System.Boolean> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_HOST_ID_REQ;
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
                var msg = new RemoveHostIdReq()
                {
                    hostId=hostId,
                    hostName=hostName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new RemoveHostIdReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<RemoveHostIdReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.arg0);
                });
                this.CallRemoteMethod(OpCode.REMOVE_HOST_ID_REQ, msg, cb);
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

