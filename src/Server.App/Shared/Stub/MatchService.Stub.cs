
//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.Protocol;
using Shared.Protocol.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace GModule.Match
{
    public partial class MatchService
    {
        [RpcMethod(ProtocolCode.JOIN_MATCH_REQ)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_SERVER_API_join_match(IMessage msg, Action<object> cb)
        {
            var _msg = (JoinMatchReq)msg;
            this.JoinMatch(_msg.uid, _msg.match_type, (code) =>
            {
                var cbMsg = new JoinMatchReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }
    }
}

