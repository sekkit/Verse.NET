
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

namespace Fenix
{

    public partial class ActorRef
    {
        public void rpc_create_actor(String typename, String name, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(OpCode.CREATE_ACTOR_REQ, new object[] { typename, name, callback });
                return;
            }
            var msg = new CreateActorReq()
            {
                typename=typename,
                name=name
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
        }
    }
}

