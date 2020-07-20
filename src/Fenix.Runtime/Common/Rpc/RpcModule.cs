using Fenix.Common;
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
        public ConcurrentDictionary<UInt32, Api> rpcTypeDic        = new ConcurrentDictionary<UInt32, Api>(); 
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
                    rpcStubDic[code] = method;
                    rpcTypeDic[code] = Api.ServerApi;
                }

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name);
                    rpcStubDic[code] = method;
                    rpcTypeDic[code] = Api.ServerOnly;
                }

                attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name);
                    rpcStubDic[code] = method;
                    rpcTypeDic[code] = Api.ClientApi;
                }
            }
        }

        public virtual void CallMethod(uint fromContainerId, uint toContainerId, Packet packet)
        {
            Type type = Global.TypeManager.GetMessageType(packet.ProtoCode);
            IMessage msg = (IMessage)MessagePackSerializer.Deserialize(type, packet.Payload); 
            bool isCallback = this.rpcDic.ContainsKey(packet.Id);
            if (isCallback)
            {
                var cmd = this.rpcDic[packet.Id];
                cmd.Callback(packet.Payload);
            }
            else
            {
                var cmd = RpcCommand.Create(fromContainerId, toContainerId, packet.FromActorId, packet.ToActorId, packet.ProtoCode, msg, this);
                cmd.Call();
            }
        }

        public virtual void CallLocalMethod(uint protoCode, object param)
        {
            var kv = new SortedDictionary<int, object>();
            foreach (var fi in param.GetType().GetFields())
            {
                var attr = fi.GetCustomAttributes<KeyAttribute>().FirstOrDefault();
                if (attr == null || attr.IntKey == null)
                    continue;
                kv.Add(attr.IntKey.Value, fi.GetValue(param));
            }
            this.rpcStubDic[protoCode].Invoke(this, kv.Values.ToArray());
        }

        public Api GetRpcType(uint protoCode)
        {
            Api api;
            if (this.rpcTypeDic.TryGetValue(protoCode, out api))
                return api;
            return Api.NoneApi;
        }

        public void Rpc(uint protoCode, uint fromContainerId, uint fromActorId, uint toActorId, IMessage msg)
        { 
            var toContainerId = Global.IdManager.GetContainerIdByActorId(toActorId);
            var peer = NetManager.Instance.GetPeerById(toContainerId);
            //Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", fromContainerId, toContainerId, fromActorId, toActorId, peer==null?"NULL":""));
            if (peer == null)
                peer = NetManager.Instance.CreatePeer(toContainerId);
             
            /*创建一个等待回调的rpc_command*/
            var cmd = RpcCommand.Create(
                fromContainerId, 
                toContainerId,
                fromActorId, 
                toActorId,
                protoCode,
                msg, 
                this);

            if(msg.HasCallback())
                this.rpcDic[cmd.Id] = cmd;

            var packet = Packet.Create(cmd.Id, protoCode, fromActorId, toActorId, MessagePackSerializer.Serialize(msg));
            peer.Send(packet);
        }

        public dynamic GetService(string name)
        {
            return Global.GetActorRef(name);
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(name);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(uid);
        }

        public ActorRef GetActorRef(string name)
        {
            return Global.GetActorRef(name);
        }
    }
}
