
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;


//using MessagePack; 
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
                var protoCode = OpCode.BIND_CLIENT_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorName, callback });
                return;
            }
            var msg = new BindClientActorReq()
            {
                actorName=actorName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new BindClientActorReq.Callback():RpcUtil.Deserialize<BindClientActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(OpCode.BIND_CLIENT_ACTOR_REQ, msg, cb);
        }

        public void CreateActor(String typename, String name, Action<DefaultErrCode, String, UInt32> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.CREATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { typename, name, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { typename, name, callback });
                return;
            }
            var msg = new CreateActorReq()
            {
                typename=typename,
                name=name
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new CreateActorReq.Callback():RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1, cbMsg.arg2);
            });
            this.CallRemoteMethod(OpCode.CREATE_ACTOR_REQ, msg, cb);
        }

        public void MigrateActor(UInt32 actorId)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = OpCode.MIGRATE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId }); 
               return;
           }
           var msg = new MigrateActorReq()
           {
                actorId=actorId
           };
           this.CallRemoteMethod(OpCode.MIGRATE_ACTOR_REQ, msg, null);
        }

        public void Register(UInt32 hostId, String hostName)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = OpCode.REGISTER_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName }); 
               return;
           }
           var msg = new RegisterReq()
           {
                hostId=hostId,
                hostName=hostName
           };
           this.CallRemoteMethod(OpCode.REGISTER_REQ, msg, null);
        }

        public void RegisterClient(UInt32 hostId, String hostName, Action<DefaultErrCode, HostInfo> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = OpCode.REGISTER_CLIENT_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { hostId, hostName, callback });
                return;
            }
            var msg = new RegisterClientReq()
            {
                hostId=hostId,
                hostName=hostName
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new RegisterClientReq.Callback():RpcUtil.Deserialize<RegisterClientReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code, cbMsg.arg1);
            });
            this.CallRemoteMethod(OpCode.REGISTER_CLIENT_REQ, msg, cb);
        }

        public void RemoveActor(UInt32 actorId)
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = OpCode.REMOVE_ACTOR_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { actorId, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { actorId }); 
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

