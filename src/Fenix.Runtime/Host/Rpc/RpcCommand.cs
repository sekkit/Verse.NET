/*
 * RpcCommand
 */

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
namespace Fenix
{
    public class RpcCommand
    {
        public ulong Id => packet.Id;

        public uint FromActorId => packet.FromActorId;

        public uint ToActorId => packet.ToActorId;

        public uint FromHostId => packet.FromHostId;

        public uint ToHostId => packet.ToHostId;

        public IMessage Msg => packet.Msg;

        public Api RpcType => mInvoker.GetRpcType(ProtoCode);

        public uint ProtoCode => packet.ProtoCode;

        protected NetworkType networkType => packet.NetType;

        Packet packet;

        //protected IPEndPoint toAddr;

        protected RpcModule mInvoker;

        protected Action<byte[]> callbackMethod;

        protected RpcCommand()
        {

        }

        public static RpcCommand Create(Packet packet, Action<byte[]> cb, RpcModule invoker)
        {
            var obj = new RpcCommand();
            obj.packet = packet;
            obj.callbackMethod = cb;
            obj.mInvoker = invoker;
            return obj;
        }

        public T ToMessage<T>() where T : IMessage
        {
            return this.Msg as T;
        }

        public void Call(Action callDone)
        {
            var args = new List<object>();
            args.Add(this.Msg);

            if (!this.Msg.HasCallback())
            {
                callDone?.Invoke();
            }
            else
            {
                var cb = new Action<object>((cbMsg) =>
                {
                    callDone?.Invoke();
                    this.mInvoker.RpcCallback(this.Id, this.ProtoCode, this.ToHostId, this.FromHostId, this.ToActorId, this.FromActorId, this.networkType, cbMsg);
                });

                args.Add(cb);
            }

            if (this.ProtoCode <= OpCode.CALL_ACTOR_METHOD)
            {
                var peer = NetManager.Instance.GetPeerById(packet.FromHostId, this.networkType);
                var context = new RpcContext(this.packet, peer);
                args.Add(context);
            }

            //if (Global.IsServer)
            //{
            if (RpcType == Api.ServerApi)
                this.mInvoker.CallLocalMethod(this.ProtoCode, args.ToArray());
            else if (RpcType == Api.ServerOnly)
                this.mInvoker.CallLocalMethod(this.ProtoCode, args.ToArray());
            //}
            //else
            //{
            if (RpcType == Api.ClientApi)
                this.mInvoker.CallLocalMethod(this.ProtoCode, args.ToArray());
            //}
        }

        public void Callback(byte[] cbData)
        {
            this.callbackMethod?.Invoke(cbData);
        }
    }


}