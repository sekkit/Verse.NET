using Fenix;
using Fenix.Common.Attributes;
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
            //发送callmethod消息到目标actor
            var msg = new JoinMatchReq()
            {
                uid=uid,
                match_type=match_type
            };

            this.CallRemoteMethod(ProtocolCode.ADD_TO_MATCH_REQ, msg);
        }
    }
}
