
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
        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API__ServerUModule__Avatar__change_name(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (__ServerUModule__Avatar__ChangeNameReq)msg;
            this.ChangeName(_msg.name, (code) =>
            {
                var cbMsg = new __ServerUModule__Avatar__ChangeNameReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY__ServerUModule__Avatar__on_match_ok(IMessage msg)
        {
            var _msg = (__ServerUModule__Avatar__OnMatchOkReq)msg;
            this.OnMatchOk();
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE__ServerUModule__Avatar__change_name(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            this.ChangeName(name, callback);
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE__ServerUModule__Avatar__on_match_ok()
        {
            this.OnMatchOk();
        }

#endif
    }
}

