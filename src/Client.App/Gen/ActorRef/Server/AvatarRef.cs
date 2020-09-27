
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;


using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
                    
//using MessagePack;
using System;
using System.Threading.Tasks;

namespace Server
{

    [RefType("Server.UModule.Avatar")]
    public partial class AvatarRef : ActorRef
    {
        public new bool isClient => false;
#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> rpc_change_name_async(dynamic name, dynamic callback=null)
#else
        public async Task<__ServerUModule__Avatar__ChangeNameReq.Callback> rpc_change_name_async(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__ServerUModule__Avatar__ChangeNameReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__ServerUModule__Avatar__ChangeNameReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<global::Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new __ServerUModule__Avatar__ChangeNameReq.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { name, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { name, _cb });
            }
            else
            {
                Action<__ServerUModule__Avatar__ChangeNameReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __ServerUModule__Avatar__ChangeNameReq()
                    {
                         name=name
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __ServerUModule__Avatar__ChangeNameReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerUModule__Avatar__ChangeNameReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void rpc_change_name(dynamic name, dynamic callback)
#else
        public void rpc_change_name(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { name, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { name, callback });
                return;
            }
            Task.Run(() => {
                var msg = new __ServerUModule__Avatar__ChangeNameReq()
                {
                    name=name
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __ServerUModule__Avatar__ChangeNameReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerUModule__Avatar__ChangeNameReq.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ, msg, cb);
            });
#endif
        }


#if FENIX_CODEGEN && !RUNTIME
        public void rpc_on_match_ok()
#else
        public void rpc_on_match_ok()
#endif
        {
#if !FENIX_CODEGEN
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] {  }); 
               return;
           }
           Task.Run(() => {
               var msg = new __ServerUModule__Avatar__OnMatchOkReq()
               {

               };
               this.CallRemoteMethod(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ, msg, null);
            });
#endif
        }

#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> rpc_test_item_api_async(dynamic callback=null)
#else
        public async Task<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback> rpc_test_item_api_async(global::System.Action callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action _cb = () =>
                {
                     var cbMsg = new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback();

                     callback?.Invoke();
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { _cb });
            }
            else
            {
                Action<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke();
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq()
                    {

                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void rpc_test_item_api(dynamic callback)
#else
        public void rpc_test_item_api(global::System.Action callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ;
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { callback });
                return;
            }
            Task.Run(() => {
                var msg = new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq()
                {

                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback>(cbData);
                    callback?.Invoke();
                });
                this.CallRemoteMethod(ProtocolCode.__SERVERUMODULE__AVATAR__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, msg, cb);
            });
#endif
        }
    }
}

