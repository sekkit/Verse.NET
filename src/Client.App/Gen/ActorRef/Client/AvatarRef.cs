
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

namespace Client
{

    [RefType("Client.Avatar")]
    public partial class AvatarRef : ActorRef
    {
        public new bool isClient => true;
#if FENIX_CODEGEN && !RUNTIME
        public async Task<dynamic> client_api_test_async(dynamic uid, dynamic callback=null)
#else
        public async Task<__Client__Avatar__ApiTestNtf.Callback> client_api_test_async(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback=null)
#endif
        {
#if FENIX_CODEGEN
#if !RUNTIME
            var t = new TaskCompletionSource<dynamic>();
#else
            var t = new TaskCompletionSource<__Client__Avatar__ApiTestNtf.Callback>();
#endif
#else
            var t = new TaskCompletionSource<__Client__Avatar__ApiTestNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new __Client__Avatar__ApiTestNtf.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, _cb });
            }
            else
            {
                Action<__Client__Avatar__ApiTestNtf.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new __Client__Avatar__ApiTestNtf()
                    {
                         uid=uid
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new __Client__Avatar__ApiTestNtf.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<__Client__Avatar__ApiTestNtf.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, msg, cb);
                 });
             }
#endif
             return await t.Task;
        }

#if FENIX_CODEGEN && !RUNTIME
        public void client_api_test(dynamic uid, dynamic callback)
#else
        public void client_api_test(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback)
#endif
        {
#if !FENIX_CODEGEN
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, callback });
                return;
            }
            Task.Run(() => {
                var msg = new __Client__Avatar__ApiTestNtf()
                {
                    uid=uid
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new __Client__Avatar__ApiTestNtf.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<__Client__Avatar__ApiTestNtf.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF, msg, cb);
            });
#endif
        }


#if FENIX_CODEGEN && !RUNTIME
        public void client_api_test2(dynamic uid, dynamic match_type)
#else
        public void client_api_test2(global::System.String uid, global::System.Int32 match_type)
#endif
        {
#if !FENIX_CODEGEN
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type }); 
               return;
           }
           Task.Run(() => {
               var msg = new __Client__Avatar__ApiTest2Ntf()
               {
                    uid=uid,
                    match_type=match_type
               };
               this.CallRemoteMethod(ProtocolCode.__CLIENT__AVATAR__API_TEST2_NTF, msg, null);
            });
#endif
        }


#if FENIX_CODEGEN && !RUNTIME
        public void client_on_sync_user(dynamic data)
#else
        public void client_on_sync_user(global::System.Byte[] data)
#endif
        {
#if !FENIX_CODEGEN
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { data, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { data }); 
               return;
           }
           Task.Run(() => {
               var msg = new __Client__Avatar__OnSyncUserNtf()
               {
                    data=data
               };
               this.CallRemoteMethod(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF, msg, null);
            });
#endif
        }
    }
}

