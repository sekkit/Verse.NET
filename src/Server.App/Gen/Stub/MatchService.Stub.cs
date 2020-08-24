
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
#if CLIENT


#endif
#if !CLIENT
        [RpcMethod(ProtocolCode.__SERVERGMODULE__MATCHSERVICE__JOIN_MATCH_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerGModule__MatchService__join_match(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerGModule__MatchService__JoinMatchReq)msg;
            this.JoinMatch(_msg.uid, _msg.match_type, (code) =>
            {
                var cbMsg = new __ServerGModule__MatchService__JoinMatchReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__MATCHSERVICE__FIND_MATCH_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY__ServerGModule__MatchService__find_match(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerGModule__MatchService__FindMatchReq)msg;
            this.FindMatch(_msg.uid, (code, user) =>
            {
                var cbMsg = new __ServerGModule__MatchService__FindMatchReq.Callback();
                cbMsg.code=code;
                cbMsg.user=user;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__MATCHSERVICE__JOIN_MATCH_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerGModule__MatchService__join_match(global::System.String uid, global::System.Int32 match_type, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            this.JoinMatch(uid, match_type, callback);
        }

        [RpcMethod(ProtocolCode.__SERVERGMODULE__MATCHSERVICE__FIND_MATCH_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE__ServerGModule__MatchService__find_match(global::System.String uid, global::System.Action<global::Shared.Protocol.ErrCode, global::Server.DataModel.Account> callback)
        {
            this.FindMatch(uid, callback);
        }

#endif
    }
}

