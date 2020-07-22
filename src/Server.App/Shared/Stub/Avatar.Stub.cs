
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
using UModule;


namespace UModule
{
    public partial class Avatar
    {
        [RpcMethod(ProtocolCode.CLIENT_API_TEST_NTF)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void _INTERNAL_CLIENT_API_client_api_test(IMessage msg, Action<object> cb)
        {
            var _msg = (ClientApiTestNtf)msg;
            this.ClientApiTest(_msg.uid, _msg.match_type, (code) =>
            {
                var cbMsg = new ClientApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }
    }
}

