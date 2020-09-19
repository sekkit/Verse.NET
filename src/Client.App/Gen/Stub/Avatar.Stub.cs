
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
#if !FENIX_CODEGEN
#if CLIENT
        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, Api.ClientApi)]
        public void CLIENT_API__Client__Avatar__api_test(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__Client__Avatar__ApiTestNtf)msg;
            this.ApiTest(_msg.uid, (code) =>
            {
                var cbMsg = new __Client__Avatar__ApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.ApiTest(_msg.uid, (global::System.Action<Shared.Protocol.ErrCode>)((code) =>
            {
                var cbMsg = new __Client__Avatar__ApiTestNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF, Api.ClientApi)]
        public void CLIENT_API__Client__Avatar__api_test2(IMessage msg)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__Client__Avatar__ApiTest2Ntf)msg;
            this.ApiTest2(_msg.uid, _msg.match_type);
#else
            dynamic _msg = msg;
            self.ApiTest2(_msg.uid, _msg.match_type);
#endif
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF, Api.ClientApi)]
        public void CLIENT_API__Client__Avatar__on_sync_user(IMessage msg)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__Client__Avatar__OnSyncUserNtf)msg;
            this.OnSyncUser(_msg.data);
            on_sync_user?.Invoke(_msg.data);
#else
            dynamic _msg = msg;
            self.OnSyncUser(_msg.data);
            on_sync_user?.Invoke(_msg.data);
#endif
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, Api.ClientApi)]
        public void CLIENT_API_NATIVE__Client__Avatar__api_test(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.ApiTest(uid, callback);
#else
            self.ApiTest(uid, callback);
#endif
        }

        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF, Api.ClientApi)]
        public void CLIENT_API_NATIVE__Client__Avatar__api_test2(global::System.String uid, global::System.Int32 match_type)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.ApiTest2(uid, match_type);
#else
            self.ApiTest2(uid, match_type);
#endif
        }

        public event Action<global::System.Byte[]> on_sync_user;
        [RpcMethod(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF, Api.ClientApi)]
        public void CLIENT_API_NATIVE__Client__Avatar__on_sync_user(global::System.Byte[] data)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.OnSyncUser(data);
            on_sync_user?.Invoke(data);
#else
            self.OnSyncUser(data);
            on_sync_user?.Invoke(data);
#endif
        }

#endif
#if !CLIENT


#endif
#endif
    }
}

