
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared;
using Shared.Protocol;
using Server.UModule;

using Server.GModule;
using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Server
{

    [RefType(typeof(MatchService))]
    public partial class MatchServiceRef : ActorRef
    {
        public void rpc_find_match(String uid, Action<ErrCode, Account> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((MatchService)Host.Instance.GetActor(this.toActorId)).FindMatch(uid, callback);
                return;
            }
            var msg = new FindMatchReq()
            {
                uid=uid
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<FindMatchReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.user);
            });
            this.CallRemoteMethod(ProtocolCode.FIND_MATCH_REQ, msg, cb);
        }

        public void rpc_join_match(String uid, Int32 match_type, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((MatchService)Host.Instance.GetActor(this.toActorId)).JoinMatch(uid, match_type, callback);
                return;
            }
            var msg = new JoinMatchReq()
            {
                uid=uid,
                match_type=match_type
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<JoinMatchReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.JOIN_MATCH_REQ, msg, cb);
        }
    }
}

