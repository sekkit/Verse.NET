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
        public async Task<BindClientActorReq.Callback> BindClientActorAsync(String actorName, Action<DefaultErrCode> callback=null)
        {
            var t = new TaskCompletionSource<BindClientActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new BindClientActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.BIND_CLIENT_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new BindClientActorReq()
                {
                actorName=actorName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new BindClientActorReq.Callback() : RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void BindClientActor(String actorName, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.BIND_CLIENT_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorName, callback });
                return;
            }
            var msg = new BindClientActorReq()
            {
                actorName=actorName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new BindClientActorReq.Callback():RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
        }

        public async Task<CreateActorReq.Callback> CreateActorAsync(String typename, String name, Action<DefaultErrCode, String, UInt64> callback=null)
        {
            var t = new TaskCompletionSource<CreateActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode, String, UInt64> _cb = (code, arg1, arg2) =>
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
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new CreateActorReq()
                {
                typename=typename,
                name=name
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new CreateActorReq.Callback() : RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void CreateActor(String typename, String name, Action<DefaultErrCode, String, UInt64> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { typename, name, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { typename, name, callback });
                return;
            }
            var msg = new CreateActorReq()
            {
                typename=typename,
                name=name
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new CreateActorReq.Callback():RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
            });
            this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
        }

        public async Task<MigrateActorReq.Callback> MigrateActorAsync(UInt64 actorId, Action<DefaultErrCode, Byte[]> callback=null)
        {
            var t = new TaskCompletionSource<MigrateActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode, Byte[]> _cb = (code, arg1) =>
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
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new MigrateActorReq()
                {
                actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new MigrateActorReq.Callback() : RpcUtil.Deserialize<MigrateActorReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void MigrateActor(UInt64 actorId, Action<DefaultErrCode, Byte[]> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, callback });
                return;
            }
            var msg = new MigrateActorReq()
            {
                actorId=actorId
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new MigrateActorReq.Callback():RpcUtil.Deserialize<MigrateActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1);
            });
            this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, cb);
        }

        public async Task<OnBeforeDisconnectNtf.Callback> OnBeforeDisconnectAsync(DisconnectReason reason, Action callback=null)
        {
            var t = new TaskCompletionSource<OnBeforeDisconnectNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action _cb = () =>
                {
                     var cbMsg = new OnBeforeDisconnectNtf.Callback();

                     callback?.Invoke();
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.ON_BEFORE_DISCONNECT_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new OnBeforeDisconnectNtf()
                {
                reason=reason
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new OnBeforeDisconnectNtf.Callback() : RpcUtil.Deserialize<OnBeforeDisconnectNtf.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.ON_BEFORE_DISCONNECT_NTF, msg, cb);
             }
             return await t.Task;
        }

        public void OnBeforeDisconnect(DisconnectReason reason, Action callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.ON_BEFORE_DISCONNECT_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { reason, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { reason, callback });
                return;
            }
            var msg = new OnBeforeDisconnectNtf()
            {
                reason=reason
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new OnBeforeDisconnectNtf.Callback():RpcUtil.Deserialize<OnBeforeDisconnectNtf.Callback>(cbData);
                callback?.Invoke();
            });
            this.CallRemoteMethod(OpCode.ON_BEFORE_DISCONNECT_NTF, msg, cb);
        }

        public async Task<ReconnectServerActorNtf.Callback> ReconnectServerActorAsync(UInt64 hostId, String hostName, String hostIP, Int32 hostPort, UInt64 actorId, String actorName, String aTypeName, Action<DefaultErrCode> callback=null)
        {
            var t = new TaskCompletionSource<ReconnectServerActorNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new ReconnectServerActorNtf.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.RECONNECT_SERVER_ACTOR_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                    var cbMsg = cbData==null ? new ReconnectServerActorNtf.Callback() : RpcUtil.Deserialize<ReconnectServerActorNtf.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, msg, cb);
             }
             return await t.Task;
        }

        public void ReconnectServerActor(UInt64 hostId, String hostName, String hostIP, Int32 hostPort, UInt64 actorId, String actorName, String aTypeName, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.RECONNECT_SERVER_ACTOR_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, callback });
                return;
            }
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
                var cbMsg = cbData==null?new ReconnectServerActorNtf.Callback():RpcUtil.Deserialize<ReconnectServerActorNtf.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, msg, cb);
        }


        public void Register(UInt64 hostId, String hostName)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = OpCode.REGISTER_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName }); 
               return;
           }
           var msg = new RegisterReq()
           {
                hostId=hostId,
                hostName=hostName
           };
           this.CallRemoteMethod(OpCode.REGISTER_REQ, msg, null);
        }

        public async Task<RegisterClientReq.Callback> RegisterClientAsync(UInt64 hostId, String hostName, Action<DefaultErrCode, HostInfo> callback=null)
        {
            var t = new TaskCompletionSource<RegisterClientReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode, HostInfo> _cb = (code, arg1) =>
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
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new RegisterClientReq()
                {
                hostId=hostId,
                hostName=hostName
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new RegisterClientReq.Callback() : RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void RegisterClient(UInt64 hostId, String hostName, Action<DefaultErrCode, HostInfo> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_CLIENT_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback });
                return;
            }
            var msg = new RegisterClientReq()
            {
                hostId=hostId,
                hostName=hostName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new RegisterClientReq.Callback():RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1);
            });
            this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
        }

        public async Task<RemoveActorReq.Callback> RemoveActorAsync(UInt64 actorId, Action<DefaultErrCode> callback=null)
        {
            var t = new TaskCompletionSource<RemoveActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new RemoveActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new RemoveActorReq()
                {
                actorId=actorId
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new RemoveActorReq.Callback() : RpcUtil.Deserialize<RemoveActorReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void RemoveActor(UInt64 actorId, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, callback });
                return;
            }
            var msg = new RemoveActorReq()
            {
                actorId=actorId
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new RemoveActorReq.Callback():RpcUtil.Deserialize<RemoveActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, cb);
        }

        public async Task<RemoveClientActorReq.Callback> RemoveClientActorAsync(UInt64 actorId, DisconnectReason reason, Action<DefaultErrCode> callback=null)
        {
            var t = new TaskCompletionSource<RemoveClientActorReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Action<DefaultErrCode> _cb = (code) =>
                {
                     var cbMsg = new RemoveClientActorReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = OpCode.REMOVE_CLIENT_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                var msg = new RemoveClientActorReq()
                {
                actorId=actorId,
                reason=reason
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null ? new RemoveClientActorReq.Callback() : RpcUtil.Deserialize<RemoveClientActorReq.Callback>(cbData);
                    _cb?.Invoke(cbMsg);
                });
                this.CallRemoteMethod(OpCode.REMOVE_CLIENT_ACTOR_REQ, msg, cb);
             }
             return await t.Task;
        }

        public void RemoveClientActor(UInt64 actorId, DisconnectReason reason, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REMOVE_CLIENT_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, reason, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId, reason, callback });
                return;
            }
            var msg = new RemoveClientActorReq()
            {
                actorId=actorId,
                reason=reason
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new RemoveClientActorReq.Callback():RpcUtil.Deserialize<RemoveClientActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.REMOVE_CLIENT_ACTOR_REQ, msg, cb);
        }
    }
}
#endif

