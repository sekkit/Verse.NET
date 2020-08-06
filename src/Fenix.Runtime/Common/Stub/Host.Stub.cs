
#if !CLIENT

//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Fenix
{
    public partial class Host
    {
        [RpcMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_reconnect_server_actor(IMessage msg, Action<IMessage> cb, RpcContext context)
        {
            var _msg = (ReconnectServerActorNtf)msg;
            this.ReconnectServerActor(_msg.hostId, _msg.hostName, _msg.hostIP, _msg.hostPort, _msg.actorId, _msg.actorName, _msg.aTypeName, (code) =>
            {
                var cbMsg = new ReconnectServerActorNtf.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.BIND_CLIENT_ACTOR_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_bind_client_actor(IMessage msg, Action<IMessage> cb, RpcContext context)
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
        public void SERVER_API_register_client(IMessage msg, Action<IMessage> cb, RpcContext context)
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
        public void SERVER_ONLY_create_actor(IMessage msg, Action<IMessage> cb, RpcContext context)
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
        public void SERVER_ONLY_migrate_actor(IMessage msg, Action<IMessage> cb, RpcContext context)
        {
            var _msg = (MigrateActorReq)msg;
            this.MigrateActor(_msg.actorId, (code, arg1) =>
            {
                var cbMsg = new MigrateActorReq.Callback();
                cbMsg.code=code;
                cbMsg.arg1=arg1;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.REGISTER_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_register(IMessage msg, RpcContext context)
        {
            var _msg = (RegisterReq)msg;
            this.Register(_msg.hostId, _msg.hostName, context);
        }

        [RpcMethod(OpCode.REMOVE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_remove_actor(IMessage msg, Action<IMessage> cb, RpcContext context)
        {
            var _msg = (RemoveActorReq)msg;
            this.RemoveActor(_msg.actorId, (code) =>
            {
                var cbMsg = new RemoveActorReq.Callback();
                cbMsg.code=code;
                cb.Invoke(cbMsg);
            }, context);
        }

        [RpcMethod(OpCode.RECONNECT_SERVER_ACTOR_NTF, Api.ClientApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CLIENT_API_NATIVE_reconnect_server_actor(UInt64 hostId, String hostName, String hostIP, Int32 hostPort, UInt64 actorId, String actorName, String aTypeName, Action<DefaultErrCode> callback, RpcContext context)
        {
            this.ReconnectServerActor(hostId, hostName, hostIP, hostPort, actorId, actorName, aTypeName, callback, context);
        }

        [RpcMethod(OpCode.BIND_CLIENT_ACTOR_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_bind_client_actor(String actorName, Action<DefaultErrCode> callback, RpcContext context)
        {
            this.BindClientActor(actorName, callback, context);
        }

        [RpcMethod(OpCode.REGISTER_CLIENT_REQ, Api.ServerApi)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_API_NATIVE_register_client(UInt64 hostId, String hostName, Action<DefaultErrCode, HostInfo> callback, RpcContext context)
        {
            this.RegisterClient(hostId, hostName, callback, context);
        }

        [RpcMethod(OpCode.CREATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_create_actor(String typename, String name, Action<DefaultErrCode, String, UInt64> callback, RpcContext context)
        {
            this.CreateActor(typename, name, callback, context);
        }

        [RpcMethod(OpCode.MIGRATE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_migrate_actor(UInt64 actorId, Action<DefaultErrCode, Byte[]> callback, RpcContext context)
        {
            this.MigrateActor(actorId, callback, context);
        }

        [RpcMethod(OpCode.REGISTER_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_register(UInt64 hostId, String hostName, RpcContext context)
        {
            this.Register(hostId, hostName, context);
        }

        [RpcMethod(OpCode.REMOVE_ACTOR_REQ, Api.ServerOnly)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SERVER_ONLY_NATIVE_remove_actor(UInt64 actorId, Action<DefaultErrCode> callback, RpcContext context)
        {
            this.RemoveActor(actorId, callback, context);
        }
   }
}

#endif
