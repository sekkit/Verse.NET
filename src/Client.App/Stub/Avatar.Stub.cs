
//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Client
{
    public partial class Avatar
    {
        [RpcMethod(ProtocolCode.API_TEST_NTF)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test(IMessage msg, Action<object> cb)
        {
            var _msg = (ApiTestNtf)msg;
            this.ApiTest(_msg.uid, _msg.match_type, (code) =>
            {
                var cbMsg = new ApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.API_TEST2_NTF)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test2(IMessage msg, Action<object> cb)
        {
            var _msg = (ApiTest2Ntf)msg;
        }
    }
}

