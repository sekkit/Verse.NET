using Fenix;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class MatchServiceRef : ActorRef
    {
        public void add_to_match(string uid, int match_type, Action<MatchCode> callback)
        {
            //发送callmethod消息到目标actor

            var msg = new AddToMatchMsg();
            msg.uid = uid;
            msg.match_type = match_type;
            this.CallActorMethod(ProtocolCode.ADD_TO_MATCH_REQ, msg);
        }
    }
}
