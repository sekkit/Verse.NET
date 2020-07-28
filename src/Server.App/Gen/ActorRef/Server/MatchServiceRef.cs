
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

using MessagePack; 
using System;

namespace Server
{

    [RefType("MatchService")]
    public partial class MatchServiceRef : ActorRef
    {
        public void rpc_find_match(String uid, Action<ErrCode, Account> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.FIND_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, callback });
                return;
            }
            var msg = new FindMatchReq()
            {
                uid=uid
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new FindMatchReq.Callback():RpcUtil.Deserialize<FindMatchReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.user);
            });
            this.CallRemoteMethod(ProtocolCode.FIND_MATCH_REQ, msg, cb);
        }

        public void rpc_join_match(String uid, Int32 match_type, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.JOIN_MATCH_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type, callback });
                return;
            }
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
        }
    }
}

