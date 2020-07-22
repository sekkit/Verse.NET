
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared.Protocol;
using UModule;

using UModule;
using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Shared
{

    [RefType(typeof(Avatar))]
    public class AvatarRef : ActorRef
    {
        public void client_on_client_api_test(String uid, Int32 match_type, Action<ErrCode> callback)
        {
            var toContainerId = Global.IdManager.GetContainerIdByActorId(this.toActorId);
            if (this.fromActor.ContainerId == toContainerId)
            {
                ((Avatar)Container.Instance.GetActor(this.toActorId)).ClientApiTest(uid, match_type, callback);
                return;
            }
            var msg = new ClientApiTestNtf()
            {
                uid=uid,
                match_type=match_type
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<ClientApiTestNtf.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.CLIENT_API_TEST_NTF, msg, cb);
        }
    }
}

