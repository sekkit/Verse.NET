
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
        public void BindClientActor(String actorName, Action<DefaultErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(OpCode.BIND_CLIENT_ACTOR_REQ, new object[] { actorName, callback });
                return;
            }
            var msg = new BindClientActorReq()
            {
                actorName=actorName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
        }

        public void CreateActor(String typename, String name, Action<DefaultErrCode, String, UInt32> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
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
                callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
            });
            this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
        }

        public void MigrateActor(UInt32 actorId)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(OpCode.MIGRATE_ACTOR_REQ, new object[] { actorId });
               return;
           }
           var msg = new MigrateActorReq()
           {
                actorId=actorId
           };
           this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, null);
        }

        public void RegisterClient(UInt32 hostId, String uniqueName, Action<Int32, HostInfo> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(OpCode.REGISTER_CLIENT_REQ, new object[] { hostId, uniqueName, callback });
                return;
            }
            var msg = new RegisterClientReq()
            {
                hostId=hostId,
                uniqueName=uniqueName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                callback?.Invoke(cbMsg.arg0, cbMsg.arg1);
            });
            this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
        }

        public void RemoveActor(UInt32 actorId)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                Host.Instance.GetActor(this.toActorId).CallLocalMethod(OpCode.REMOVE_ACTOR_REQ, new object[] { actorId });
               return;
           }
           var msg = new RemoveActorReq()
           {
                actorId=actorId
           };
           this.CallRemoteMethod(OpCode.REMOVE_ACTOR_REQ, msg, null);
        }
    }
}

