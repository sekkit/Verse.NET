
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

namespace Server
{

    [RefType("Server.UModule.Avatar")]
    public partial class AvatarRef : ActorRef
    {
        public void rpc_change_name(String name, Action<ErrCode> callback)
        {
            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
            if (this.FromHostId == toHostId)
            {
                var protoCode = ProtocolCode.CHANGE_NAME_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { name, callback, context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] { name, callback });
                return;
            }
            var msg = new ChangeNameReq()
            {
                name=name
            };
            var cb = new Action<byte[]>((cbData) => {
                var cbMsg = cbData==null?new ChangeNameReq.Callback():RpcUtil.Deserialize<ChangeNameReq.Callback>(cbData);
                callback?.Invoke(cbMsg.code);
            });
            this.CallRemoteMethod(ProtocolCode.CHANGE_NAME_REQ, msg, cb);
        }

        public void rpc_on_match_ok()
        {
           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);
           if (this.FromHostId == toHostId)
           {
                var protoCode = ProtocolCode.ON_MATCH_OK_REQ;
                if (protoCode < OpCode.CALL_ACTOR_METHOD)
                {
                    var peer = NetManager.Instance.GetPeerById(this.FromHostId, this.NetType);
                    var context = new RpcContext(null, peer);
                    Global.Host.CallMethodWithParams(protoCode, new object[] { context });
                }
                else
                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] {  }); 
               return;
           }
           var msg = new OnMatchOkReq()
           {

           };
           this.CallRemoteMethod(ProtocolCode.ON_MATCH_OK_REQ, msg, null);
        }
    }
}

