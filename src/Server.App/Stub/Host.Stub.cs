
//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Fenix
{
    public partial class Host
    {
        [RpcMethod(OpCode.CREATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_create_actor(IMessage msg, Action<object> cb)
        {
            var _msg = (CreateActorReq)msg;
            this.CreateActor(_msg.typename, _msg.name, (code) =>
            {
                var cbMsg = new CreateActorReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            });
        }
    }
}

