
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared;
using Shared.Protocol;
using Server.UModule;

using Client;
using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Client
{

    [RefType(typeof(Avatar))]
    public partial class AvatarRef : ActorRef
    {
        public void client_on_api_test(String uid, Int32 match_type, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.fromActor.HostId == toHostId)
            {
                ((Avatar)Host.Instance.GetActor(this.toActorId)).ApiTest(uid, match_type, callback);
                return;
            }
            var msg = new ApiTestNtf()
            {
                uid=uid,
                match_type=match_type
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<ApiTestNtf.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.API_TEST_NTF, msg, cb);
        }

        public void client_on_api_test2(String uid, Int32 match_type)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
           if (this.fromActor.HostId == toHostId)
           {
               ((Avatar)Host.Instance.GetActor(this.toActorId)).ApiTest2(uid, match_type);
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

