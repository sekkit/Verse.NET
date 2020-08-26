
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
        public async Task<__ServerUModule__Avatar__ChangeNameReq.Callback> rpc_change_name_async(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback=null)
        {
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
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
             return await t.Task;
        }

        public void rpc_change_name(global::System.String name, global::System.Action<global::Shared.Protocol.ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
        }


        public void rpc_on_match_ok()
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
        }

        public async Task<__ServerUModule__Avatar__M__ServerUModule__ItemModule__TestItemApiReq.Callback> rpc_test_item_api_async(global::System.Action callback=null)
        {
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
                var protoCode = ProtocolCode.__ServerUModule__Avatar__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                    this.CallRemoteMethod(ProtocolCode.__ServerUModule__Avatar__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, msg, cb);
                 });
             }
             return await t.Task;
        }

        public void rpc_test_item_api(global::System.Action callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__ServerUModule__Avatar__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
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
                this.CallRemoteMethod(ProtocolCode.__ServerUModule__Avatar__M__SERVERUMODULE__ITEMMODULE__TEST_ITEM_API_REQ, msg, cb);
            });
        }
    }
}

