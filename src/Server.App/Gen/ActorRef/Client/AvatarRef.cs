
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
        public async Task<ApiTestNtf.Callback> client_api_test_async(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback=null)
        {
            var t = new TaskCompletionSource<ApiTestNtf.Callback>();
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                global::System.Action<Shared.Protocol.ErrCode> _cb = (code) =>
                {
                     var cbMsg = new ApiTestNtf.Callback();
                     cbMsg.code=code;
                     callback?.Invoke(cbMsg.code);
                     t.TrySetResult(cbMsg);
                }; 
                var protoCode = ProtocolCode.API_TEST_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, _cb, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, _cb });
            }
            else
            {
                Action<ApiTestNtf.Callback> _cb = (cbMsg) =>
                {
                    callback?.Invoke(cbMsg.code);
                    t.TrySetResult(cbMsg);
                };
                await Task.Run(() => {
                    var msg = new ApiTestNtf()
                    {
                         uid=uid
                    };
                    var cb = new Action<byte[]>((cbData) => {
                        var cbMsg = cbData==null ? new ApiTestNtf.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<ApiTestNtf.Callback>(cbData);
                        _cb?.Invoke(cbMsg);
                    });
                    this.CallRemoteMethod(ProtocolCode.API_TEST_NTF, msg, cb);
                 });
             }
             return await t.Task;
        }

        public void client_api_test(global::System.String uid, global::System.Action<Shared.Protocol.ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.API_TEST_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, callback });
                return;
            }
            Task.Run(() => {
                var msg = new ApiTestNtf()
                {
                    uid=uid
                };
                var cb = new Action<byte[]>((cbData) => {
                    var cbMsg = cbData==null?new ApiTestNtf.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<ApiTestNtf.Callback>(cbData);
                    callback?.Invoke(cbMsg.code);
                });
                this.CallRemoteMethod(ProtocolCode.API_TEST_NTF, msg, cb);
            });
        }


        public void client_api_test2(global::System.String uid, global::System.Int32 match_type)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.API_TEST2_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = Global.NetManager.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type }); 
               return;
           }
           Task.Run(() => {
               var msg = new ApiTest2Ntf()
               {
                    uid=uid,
                    match_type=match_type
               };
               this.CallRemoteMethod(ProtocolCode.API_TEST2_NTF, msg, null);
            });
        }
    }
}

