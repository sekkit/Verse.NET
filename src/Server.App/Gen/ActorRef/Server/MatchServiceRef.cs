
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

    [RefType("MatchService")]
    public partial class MatchServiceRef : ActorRef
    {
        public async Task<FindMatchReq.Callback> rpc_find_match_async(global::System.String uid, global::System.Action<global::Shared.Protocol.ErrCode, global::Server.DataModel.Account> callback=null)
        {
            var t = new TaskCompletionSource<FindMatchReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode, global::Server.DataModel.Account> _cb = (code, user) =>
                {
                     var cbMsg = new FindMatchReq.Callback();
                     cbMsg.code=code;
                     cbMsg.user=user;
                     callback?.Invoke(cbMsg.code, cbMsg.user);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.FIND_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, _cb });
            }
            else
            {
                Action<FindMatchReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code, cbMsg.user);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new FindMatchReq()
                    {
                         uid=uid
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new FindMatchReq.Callback() : RpcUtil.Deserialize<FindMatchReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.FIND_MATCH_REQ, msg, cb);
                 });
             }
             return await t.Task;
        }

        public void rpc_find_match(global::System.String uid, global::System.Action<global::Shared.Protocol.ErrCode, global::Server.DataModel.Account> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.FIND_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, callback });
                return;
            }
            Task.Run(() => {
                var msg = new FindMatchReq()
                {
                    uid=uid
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new FindMatchReq.Callback():RpcUtil.Deserialize<FindMatchReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code, cbMsg.user);
                });
                this.CallRemoteMethod(ProtocolCode.FIND_MATCH_REQ, msg, cb);
            });
        }

        public async Task<JoinMatchReq.Callback> rpc_join_match_async(global::System.String uid, global::System.Int32 match_type, global::System.Action<global::Shared.Protocol.ErrCode> callback=null)
        {
            var t = new TaskCompletionSource<JoinMatchReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new JoinMatchReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.JOIN_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type, _cb });
            }
            else
            {
                Action<JoinMatchReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new JoinMatchReq()
                    {
                         uid=uid,
                         match_type=match_type
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new JoinMatchReq.Callback() : RpcUtil.Deserialize<JoinMatchReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.JOIN_MATCH_REQ, msg, cb);
                 });
             }
             return await t.Task;
        }

        public void rpc_join_match(global::System.String uid, global::System.Int32 match_type, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.JOIN_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type, callback });
                return;
            }
            Task.Run(() => {
                var msg = new JoinMatchReq()
                {
                    uid=uid,
                    match_type=match_type
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new JoinMatchReq.Callback():RpcUtil.Deserialize<JoinMatchReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.JOIN_MATCH_REQ, msg, cb);
            });
        }
    }
}

