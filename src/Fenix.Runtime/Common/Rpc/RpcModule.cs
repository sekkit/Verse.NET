using DotNetty.Codecs.Redis;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection; 
using System.Text;
using System.Threading.Tasks;
using static Fenix.Common.RpcUtil;

namespace Fenix
{
    public class RpcModule
    {
        public ConcurrentDictionary<UInt64, RpcCommand> rpcDic     = new ConcurrentDictionary<UInt64, RpcCommand>();
        public static ConcurrentDictionary<UInt32, Api> rpcTypeDic        = new ConcurrentDictionary<UInt32, Api>(); 
        public ConcurrentDictionary<UInt32, MethodInfo> rpcStubDic = new ConcurrentDictionary<UInt32, MethodInfo>(); 

        public RpcModule()
        {
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attrs = method.GetCustomAttributes(typeof(ServerApiAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name); 
                    rpcTypeDic[code] = Api.ServerApi;
                }

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name); 
                    rpcTypeDic[code] = Api.ServerOnly;
                }

                attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name); 
                    rpcTypeDic[code] = Api.ClientApi;
                }

                attrs = method.GetCustomAttributes(typeof(RpcMethodAttribute));
                if (attrs.Count() > 0)
                {
                    var attr = (RpcMethodAttribute)attrs.First();
                    uint code = attr.Code;
                    rpcStubDic[code] = method;
                }
            }
        }

        public virtual void CallMethod(uint fromContainerId, uint toContainerId, Packet packet)
        {
            bool isCallback = this.rpcDic.ContainsKey(packet.Id);
            
            if (isCallback)
            { 
                var cmd = this.rpcDic[packet.Id];
                this.rpcDic.TryRemove(packet.Id, out var _);
                cmd.Callback(packet.Payload);
            }
            else
            {
                Type type = Global.TypeManager.GetMessageType(packet.ProtoCode);
                IMessage msg = (IMessage)Basic.Deserialize(type, packet.Payload);
                var cmd = RpcCommand.Create(packet.Id, fromContainerId, toContainerId, packet.FromActorId, packet.ToActorId, packet.ProtoCode, msg,
                    (cbData) => { },
                    this) ; 
                cmd.Call(()=> {
                    this.rpcDic.TryRemove(cmd.Id, out var _);
                });
            }
        }

        public virtual void CallLocalMethod(uint protoCode, object[] args)
        {
            this.rpcStubDic[protoCode].Invoke(this, args);
        }

        public Api GetRpcType(uint protoCode)
        {
            Api api;
            if (rpcTypeDic.TryGetValue(protoCode, out api))
                return api;
            return Api.NoneApi;
        }

        public void Rpc(uint protoCode, uint fromContainerId, uint fromActorId, uint toActorId, IMessage msg, Action<byte[]> cb)
        { 
            var toContainerId = Global.IdManager.GetContainerIdByActorId(toActorId);

            /*创建一个等待回调的rpc_command*/
            var cmd = RpcCommand.Create(
                Basic.GenID64(),
                fromContainerId,
                toContainerId,
                fromActorId,
                toActorId,
                protoCode,
                msg,
                (data) => cb(data),
                this);

            var packet = Packet.Create(cmd.Id, cmd.ProtoCode, cmd.FromActorId, cmd.ToActorId, Basic.Serialize(msg));

            //如果是同进程，则本地调用
            if (fromContainerId == toContainerId)
            {
                var toActor = Container.Instance.GetActor(toActorId);

                if (msg.HasCallback())
                    this.rpcDic[cmd.Id] = cmd; 
                toActor.CallMethod(fromContainerId, toContainerId, packet);
                return;
            }

            //否则通过网络调用
            var peer = NetManager.Instance.GetPeerById(toContainerId);
            //Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", fromContainerId, toContainerId, fromActorId, toActorId, peer==null?"NULL":""));
            if (peer == null)
                peer = NetManager.Instance.CreatePeer(toContainerId);
              
            if(msg.HasCallback())
                this.rpcDic[cmd.Id] = cmd; 
            
            peer.Send(packet);
        }

        public void RpcCallback(ulong protoId, uint protoCode, uint fromActorId, uint toActorId, object cbMsg)
        { 
            var toContainerId = Global.IdManager.GetContainerIdByActorId(toActorId);
            var fromContainerId = Global.IdManager.GetContainerIdByActorId(fromActorId);

            var packet = Packet.Create(protoId, protoCode, fromActorId, toActorId, Basic.Serialize(cbMsg));

            //如果是同进程，则本地调用
            if (fromContainerId == toContainerId)
            {
                var toActor = Container.Instance.GetActor(toActorId);
                toActor.CallMethod(fromContainerId, toContainerId, packet);
                return;
            }

            //否则通过网络调用
            var peer = NetManager.Instance.GetPeerById(toContainerId);

            Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", Container.Instance.Id, toContainerId, 
                Global.TypeManager.GetActorType(fromActorId).Name, Global.TypeManager.GetActorType(toActorId).Name, 
                peer==null?"NULL":""));

            if (peer == null)
                peer = NetManager.Instance.CreatePeer(toContainerId);

            
            peer.Send(packet);
        }
    }
}
