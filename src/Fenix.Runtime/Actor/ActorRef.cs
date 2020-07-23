using Fenix.Common;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fenix
{
    public partial class ActorRef
    {
        public uint FromHostId => fromHost.Id;

        protected Host fromHost;

        protected Actor fromActor;

        protected uint toHostId;

        protected uint toActorId;

        protected IPEndPoint toAddr; 

        public static ActorRef Create(uint toHostId, uint toActorId, Type refType, Actor fromActor, Host fromHost, IPEndPoint toPeerEP=null)
        {
            //要检测一下fromActor.HostId和fromHost.Id是不是相等
            if(fromActor!=null && fromActor.HostId != fromHost.Id)
            {
                Log.Error(string.Format("actor_and_host_id_unmatch {0} {1}", fromActor.UniqueName, fromHost.UniqueName));
                return null;
            }
            //uint toActorId = Basic.GenID32FromName(toActorName);
            //var refType = Global.TypeManager.GetRefType(toActorTypeName);

            IPEndPoint toAddr = null;
            if (toPeerEP != null)
                toAddr = toPeerEP;
            else
            {
                if(toHostId != 0)
                    toAddr = Basic.ToAddress(Global.IdManager.GetHostAddr(toHostId));
                else if(toActorId != 0)
                    toAddr = Basic.ToAddress(Global.IdManager.GetHostAddrByActorId(toActorId));
            }

            var obj = (ActorRef)Activator.CreateInstance(refType);
            obj.toHostId = toHostId;
            obj.toActorId = toActorId;
            obj.fromActor = fromActor;
            obj.fromHost = fromHost;
            obj.toAddr = toAddr;
            return obj;
        }

        public void CallRemoteMethod(uint protocolCode, IMessage msg, Action<byte[]> cb)
        {
            //如果protocode是client_api，则用kcp
            //否则都是tcp
            //暂定如此

            var api = Global.TypeManager.GetApiType(protocolCode);
            var netType = NetworkType.TCP;
            if (api == Common.Attributes.Api.ClientApi)
                netType = NetworkType.KCP;
            if (fromActor != null)
                fromActor.Rpc(protocolCode, FromHostId, fromActor.Id, toHostId, this.toActorId, toAddr, netType, msg, cb);
            else
                fromHost.Rpc(protocolCode, FromHostId, 0, toHostId, this.toActorId, toAddr, netType, msg, cb);
        }
        
        public void CreateActor(string actorTypeName, string uid, Action<DefaultErrCode> callback)
        {
            //必须由Host来创建actor
            var hostAddr = Global.IdManager.GetHostAddr(toHostId);

            var msg = new CreateActorReq();
            msg.typeName = actorTypeName;
            msg.name = uid;

            this.CallRemoteMethod((uint)ProtoCode.CREATE_ACTOR, msg, (cbData) =>
            {
                var cbMsg = RpcUtil.Deserialize<CreateActorReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
        }
    }
}