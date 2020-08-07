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

