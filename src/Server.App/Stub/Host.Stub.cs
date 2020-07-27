
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

        [RpcMethod(OpCode.REGISTER_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_register(IMessage msg, RpcContext context)
        {
            var _msg = (RegisterReq)msg;
            this.Register(_msg.hostId, _msg.hostName, context);
        }

        [RpcMethod(OpCode.REGISTER_CLIENT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_register_client(IMessage msg, Action<object> cb, RpcContext context)
        {
            var _msg = (RegisterClientReq)msg;
            this.RegisterClient(_msg.hostId, _msg.hostName, (code, arg1) =>
            {
                var cbMsg = new RegisterClientReq.Callback();
                cbMsg.code=code;
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

        [RpcMethod(OpCode.BIND_CLIENT_ACTOR_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_bind_client_actor(String actorName, Action<DefaultErrCode> callback, RpcContext context)
        {
            this.BindClientActor(actorName, callback, context);
        }

        [RpcMethod(OpCode.REGISTER_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_register(UInt32 hostId, String hostName, RpcContext context)
        {
            this.Register(hostId, hostName, context);
        }

        [RpcMethod(OpCode.REGISTER_CLIENT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_register_client(UInt32 hostId, String hostName, Action<DefaultErrCode, HostInfo> callback, RpcContext context)
        {
            this.RegisterClient(hostId, hostName, callback, context);
        }

        [RpcMethod(OpCode.CREATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_create_actor(String typename, String name, Action<DefaultErrCode, String, UInt32> callback, RpcContext context)
        {
            this.CreateActor(typename, name, callback, context);
        }

        [RpcMethod(OpCode.MIGRATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_migrate_actor(UInt32 actorId, RpcContext context)
        {
            this.MigrateActor(actorId, context);
        }

        [RpcMethod(OpCode.REMOVE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_remove_actor(UInt32 actorId, RpcContext context)
        {
            this.RemoveActor(actorId, context);
        }
   }
}

