
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


namespace Server.UModule
{
    public partial class Avatar
    {
#if CLIENT


#endif
#if !CLIENT
        [RpcMethod(ProtocolCode.CHANGE_NAME_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_change_name(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (ChangeNameReq)msg;
            this.ChangeName(_msg.name, (code) =>
            {
                var cbMsg = new ChangeNameReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.ON_MATCH_OK_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_on_match_ok(IMessage msg)
        {
            var _msg = (OnMatchOkReq)msg;
            this.OnMatchOk();
        }

        [RpcMethod(ProtocolCode.CHANGE_NAME_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_change_name(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            this.ChangeName(name, callback);
        }

        [RpcMethod(ProtocolCode.ON_MATCH_OK_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_on_match_ok()
        {
            this.OnMatchOk();
        }

#endif
    }
}

