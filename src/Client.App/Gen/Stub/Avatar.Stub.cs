
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
#if CLIENT
        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test(IMessage msg, Action<IMessage> cb)
        {
            var _msg = (ApiTestNtf)msg;
            this.ApiTest(_msg.uid, (code) =>
            {
                var cbMsg = new ApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_api_test2(IMessage msg)
        {
            var _msg = (ApiTest2Ntf)msg;
            this.ApiTest2(_msg.uid, _msg.match_type);
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_on_sync_user(IMessage msg)
        {
            var _msg = (OnSyncUserNtf)msg;
            this.OnSyncUser(_msg.data);
            on_sync_user?.Invoke(_msg.data);
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_api_test(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback)
        {
            this.ApiTest(uid, callback);
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_api_test2(global::System.String uid, global::System.Int32 match_type)
        {
            this.ApiTest2(uid, match_type);
        }

        public event Action<global::System.Byte[]> on_sync_user;
        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_on_sync_user(global::System.Byte[] data)
        {
            this.OnSyncUser(data);
            on_sync_user?.Invoke(data);
        }

#endif
#if !CLIENT


#endif
    }
}

