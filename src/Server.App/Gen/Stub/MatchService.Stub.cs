
//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;

using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
using Server.DataModel;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Server.GModule
{
    public partial class MatchService
    {
        [RpcMethod(ProtocolCode.JOIN_MATCH_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_join_match(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (JoinMatchReq)msg;
            this.JoinMatch(_msg.uid, _msg.match_type, (code) =>
            {
                var cbMsg = new JoinMatchReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.FIND_MATCH_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_find_match(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (FindMatchReq)msg;
            this.FindMatch(_msg.uid, (code, user) =>
            {
                var cbMsg = new FindMatchReq.Callback();
                cbMsg.code=code;
                cbMsg.user=user;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.JOIN_MATCH_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_join_match(String uid, Int32 match_type, Action<ErrCode> callback)
        {
            this.JoinMatch(uid, match_type, callback);
        }

        [RpcMethod(ProtocolCode.FIND_MATCH_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_find_match(String uid, Action<ErrCode, Server.DataModel.Account> callback)
        {
            this.FindMatch(uid, callback);
        }
   }
}

