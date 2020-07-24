
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
        [RpcMethod(OpCode.BIND_CLIENT_ACTOR_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_bind_client_actor(IMessage msg, Action<object> cb, RpcContext context)
        {
            var _msg = (BindClientActorReq)msg;
            this.BindClientActor(_msg.actorName, (code) =>
            {
                var cbMsg = new BindClientActorReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.REGISTER_CLIENT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_register_client(IMessage msg, Action<object> cb, RpcContext context)
        {
            var _msg = (RegisterClientReq)msg;
            this.RegisterClient(_msg.hostId, _msg.uniqueName, (arg0, arg1) =>
            {
                var cbMsg = new RegisterClientReq.Callback();
                cbMsg.arg0=arg0;
                cbMsg.arg1=arg1;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.CREATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_create_actor(IMessage msg, Action<object> cb, RpcContext context)
        {
            var _msg = (CreateActorReq)msg;
            this.CreateActor(_msg.typename, _msg.name, (code, arg1, arg2) =>
            {
                var cbMsg = new CreateActorReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cbMsg.arg2=arg2;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.MIGRATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_migrate_actor(IMessage msg, RpcContext context)
        {
            var _msg = (MigrateActorReq)msg;
            this.MigrateActor(_msg.actorId, context);
        }

        [RpcMethod(OpCode.REMOVE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_remove_actor(IMessage msg, RpcContext context)
        {
            var _msg = (RemoveActorReq)msg;
            this.RemoveActor(_msg.actorId, context);
        }
    }
}

