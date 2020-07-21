//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
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
        public void _api_join_match_req(IMessage msg, Action<object> cb)
        {
            var _msg = (JoinMatchReq)msg;

            this.JoinMatch(_msg.uid, _msg.match_type.Value, (code) =>
            {
                var cbMsg = new JoinMatchReq.Callback();
                cbMsg.code = code;
                cb.Invoke(cbMsg);
            });
        }
    }
}
