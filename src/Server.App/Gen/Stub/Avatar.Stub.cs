
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
#if !FENIX_CODEGEN
#if CLIENT


#endif
#if !CLIENT
        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, Api.ServerApi)]
        public void SERVER_API__ServerUModule__Avatar__change_name(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerUModule__Avatar__ChangeNameReq)msg;
            this.ChangeName(_msg.name, (code) =>
            {
                var cbMsg = new __ServerUModule__Avatar__ChangeNameReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.ChangeName(_msg.name, new Action<dynamic>((code) =>
            {
                dynamic cbMsg = new __ServerUModule__Avatar__ChangeNameReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, Api.ServerApi)]
        public void SERVER_API__ServerUModule__Avatar__M__ServerUModule__ItemModule__test_item_api(IMessage msg, Action<IMessage> cb)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq)msg;
            this.GetModule<ItemModule>().TestItemApi( () =>
            {
                var cbMsg = new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback();

                cb.Invoke(cbMsg);
            });
#else

            dynamic _msg = msg;
            self.GetModule<ItemModule>().TestItemApi( new Action(() =>
            {
                dynamic cbMsg = new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback();

                cb.Invoke(cbMsg);
            }));
#endif

        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ, Api.ServerOnly)]
        public void SERVER_ONLY__ServerUModule__Avatar__on_match_ok(IMessage msg)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            var _msg = (__ServerUModule__Avatar__OnMatchOkReq)msg;
            this.OnMatchOk();
#else
            dynamic _msg = msg;
            self.OnMatchOk();
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerUModule__Avatar__change_name(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.ChangeName(name, callback);
#else
            self.ChangeName(name, callback);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, Api.ServerApi)]
        public void SERVER_API_NATIVE__ServerUModule__Avatar__M__ServerUModule__ItemModule__test_item_api(global::System.Action callback)
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.GetModule<ItemModule>().TestItemApi(callback);
#else
            self.GetModule<ItemModule>().TestItemApi(callback);
#endif
        }

        [RpcMethod(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ, Api.ServerOnly)]
        public void SERVER_ONLY_NATIVE__ServerUModule__Avatar__on_match_ok()
        {
#if ENABLE_IL2CPP || !DEBUG || RUNTIME
            this.OnMatchOk();
#else
            self.OnMatchOk();
#endif
        }

#endif
#endif
    }
}

