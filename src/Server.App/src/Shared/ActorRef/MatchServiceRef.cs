using Fenix;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using GModule.Match;
using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Shared
{
    [RefType(typeof(MatchService))]
    public class MatchServiceRef : ActorRef
    {
        public void rpc_join_match(string uid, int match_type, Action<MatchCode> callback)
        {
            var toContainerId = Global.IdManager.GetContainerIdByActorId(this.toActorId);
            if (this.fromActor.ContainerId == toContainerId)
            {
                ((MatchService)Container.Instance.GetActor(this.toActorId)).JoinMatch(uid, match_type, (code) => { callback(code); });
                return;
            }

            //发送callmethod消息到目标actor
            var msg = new JoinMatchReq()
            {
                uid=uid,
                match_type=match_type
            };

            var cb = new Action<byte[]>((cbData) => { 
                var cbMsg = Basic.Deserialize<JoinMatchReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });

            this.CallRemoteMethod(ProtocolCode.JOIN_MATCH_REQ, msg, cb);
        }
    }
}
