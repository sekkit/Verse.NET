
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

using MessagePack; 
using System;

namespace Client
{

    [RefType("Client.Avatar")]
    public partial class AvatarRef : ActorRef
    {
        public void client_on_api_test(String uid, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.API_TEST_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, callback });
                return;
            }
            var msg = new ApiTestNtf()
            {
                uid=uid
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new ApiTestNtf.Callback():RpcUtil.Deserialize<ApiTestNtf.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.API_TEST_NTF, msg, cb);
        }

        public void client_on_api_test2(String uid, Int32 match_type)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.API_TEST2_NTF;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { uid, match_type, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { uid, match_type }); 
               return;
           }
           var msg = new ApiTest2Ntf()
           {
                uid=uid,
                match_type=match_type
           };
           this.CallRemoteMethod(ProtocolCode.API_TEST2_NTF, msg, null);
        }
    }
}

