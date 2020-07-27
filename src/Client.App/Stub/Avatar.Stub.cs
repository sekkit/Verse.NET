
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Client
{
    public partial class Avatar
    {
        [RpcMethod(ProtocolCode.API_TEST_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test(IMessage msg, Action<object> cb)
        {
            var _msg = (ApiTestNtf)msg;
            this.ApiTest(_msg.uid, (code) =>
            {
                var cbMsg = new ApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.API_TEST2_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test2(IMessage msg)
        {
            var _msg = (ApiTest2Ntf)msg;
            this.ApiTest2(_msg.uid, _msg.match_type);
        }

        [RpcMethod(ProtocolCode.API_TEST_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_api_test(String uid, Action<ErrCode> callback)
        {
            this.ApiTest(uid, callback);
        }

        [RpcMethod(ProtocolCode.API_TEST2_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_api_test2(String uid, Int32 match_type)
        {
            this.ApiTest2(uid, match_type);
        }
   }
}

