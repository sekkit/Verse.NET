 
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
//using MessagePack;
using System;
using System.Collections.Concurrent; 
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Fenix
{ 
    public abstract class Entity//: IMessage
    { 
        public ulong Id { get; set; }
         
        public string UniqueName { get; set; }
         
        public bool IsAlive { get; set; } = true;
         
        public ConcurrentDictionary<UInt64, RpcCommand> rpcDic     = new ConcurrentDictionary<UInt64, RpcCommand>();
         
        public ConcurrentDictionary<UInt32, MethodInfo> rpcStubDic = new ConcurrentDictionary<UInt32, MethodInfo>();
         
        public ConcurrentDictionary<UInt32, MethodInfo> rpcNativeStubDic = new ConcurrentDictionary<uint, MethodInfo>();
         
        private ConcurrentDictionary<ulong, Timer> mTimerDic = new ConcurrentDictionary<ulong, Timer>();

        public Entity()
        {
            string ns = GetType().Namespace;
            string tname = GetType().Name;
            bool isHost = GetType().FullName == "Fenix.Host";
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attrs = method.GetCustomAttributes(typeof(ServerApiAttribute));
                if (attrs.Count() > 0)
                {
                    string protoCode = (isHost? NameToProtoCode(method.Name) : NameToProtoCode(ns, tname, method.Name)) + "_" + GetApiMessagePostfix(Api.ServerApi).ToUpper();
                    uint code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ServerApi);
                }

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    string protoCode = (isHost ? NameToProtoCode(method.Name) : NameToProtoCode(ns, tname, method.Name)) + "_" + GetApiMessagePostfix(Api.ServerOnly).ToUpper();
                    uint code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ServerOnly);
                }

                attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                if (attrs.Count() > 0)
                {
                    string protoCode = (isHost ? NameToProtoCode(method.Name) : NameToProtoCode(ns, tname, method.Name)) + "_" + GetApiMessagePostfix(Api.ClientApi).ToUpper();
                    uint code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ClientApi);
                }

                attrs = method.GetCustomAttributes(typeof(RpcMethodAttribute));
                if (attrs.Count() > 0)
                {
                    var attr = (RpcMethodAttribute)attrs.First();
                    uint code = attr.Code;
                    Api api = attr.Api; 
                    Global.TypeManager.RegisterApi(code, api);
                    if (method.Name.Contains("_NATIVE_"))
                        rpcNativeStubDic[code] = method;
                    else
                        rpcStubDic[code] = method;
                }
            }

            this.AddRepeatedTimer(1000, 2000, CheckRpc);
        }

        static string GetApiMessagePostfix(Api api)
        {
            if (api == Api.ClientApi)
                return "Ntf";
            if (api == Api.ServerApi)
                return "Req";
            if (api == Api.ServerOnly)
                return "Req";
            return "";
        }

        static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        static string NameToProtoCode(string apiName)
        {
            var parts = SplitCamelCase(apiName);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToUpper();
            return string.Format("{0}", string.Join("_", parts));
        }

        static string NameToProtoCode(string ns, string entityName, string apiName)
        {
            var parts = SplitCamelCase(apiName);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToUpper();
            return string.Format("__{0}__{1}__{2}", ns.Replace(".", "").ToUpper(), entityName.ToUpper(), string.Join("_", parts));
        }

        public void AddCallbackRpc(RpcCommand cmd)
        {
            rpcDic[cmd.Id] = cmd;
            Global.IdManager.RegisterRpcId(cmd.Id, this.Id);
        }

        public void RemoveRpc(ulong rpcId)
        {
            Global.IdManager.RemoveRpcId(rpcId);
            rpcDic.TryRemove(rpcId, out var _);
        }

        public RpcCommand GetRpc(ulong rpcId)
        {
            rpcDic.TryGetValue(rpcId, out var cmd);
            return cmd;
        }

        public virtual void CallMethod(Packet packet)
        {
            bool isCallback = rpcDic.ContainsKey(packet.Id); 
            if (isCallback)
            { 
                if(!rpcDic.TryGetValue(packet.Id, out var cmd))
                {
                    Log.Error("rpc_id_not_found", packet.Id, packet.ProtoCode, packet.MsgType, packet.NetType);
                    return;
                }
                 
                RemoveRpc(cmd.Id);
                cmd.Callback(packet.Payload);
            }
            else
            { 
                var cmd = RpcCommand.Create(packet, null, this) ; 
                cmd.Call(()=> {
                    RemoveRpc(cmd.Id);
                });
            }
        }

        public virtual void CallMethodWithParams(uint protoCode, object[] args)
        {
            try
            {
                rpcNativeStubDic[protoCode].Invoke(this, args);
            }
            catch(Exception ex)
            {
                Log.Error(this.UniqueName);
                Log.Error(ex.ToString());
            }
        }

        public virtual void CallMethodWithMsg(uint protoCode, object[] args)
        {
            try
            {
                rpcStubDic[protoCode].Invoke(this, args);
            }
            catch (Exception ex)
            {
                Log.Error(this.UniqueName);
                Log.Error(ex.ToString());
            }
        }

        public void Rpc(uint protoCode, ulong fromHostId, ulong fromActorId, ulong toHostId, ulong toActorId, 
            IPEndPoint toPeerAddr, NetworkType netType, IMessage msg, Action<byte[]> cb)
        { 
            var packet = Packet.Create(Basic.GenID64(), protoCode, fromHostId, toHostId, fromActorId, toActorId, netType, msg.GetType(), RpcUtil.Serialize(msg));

            /*创建一个等待回调的rpc_command*/
            var cmd = RpcCommand.Create(
                packet,
                (data) => { RemoveRpc(packet.Id); cb?.Invoke(data);  },
                this); 

            //如果是同进程，则本地调用
            if (fromHostId == toHostId)
            {
                var toActor = Global.Host.GetActor(toActorId);

                if (msg.HasCallback())
                {
                    AddCallbackRpc(cmd);
                }
                toActor.CallMethod(packet);
                return;
            }

            //否则通过网络调用
            var peer = Global.NetManager.GetPeerById(toHostId, netType);
            //Log.Info(string.Format("{0} {1} {2} {3} {4}", fromHostId, toHostId, fromActorId, toActorId, peer==null?"NULL":""));
            if (peer == null)
            {
                Log.Warn(string.Format("Rpc:cannot_find_peer_and_create {0} => {1} ({2})", fromHostId, toHostId, netType));
                peer = Global.NetManager.CreatePeer(toHostId, toPeerAddr, netType);
            }
  
            if (peer == null || !peer.IsActive)
            {
                Log.Error(string.Format("Rpc:peer disconnected {0}", toHostId), toPeerAddr, netType);
                //这里可以尝试把global以及redis状态清空
                if (peer == null)
                    Global.NetManager.RemovePeerId(toHostId);
                else
                    Global.NetManager.Deregister(peer);
                cb?.Invoke(null);
                return;
            }

            if (msg.HasCallback())
            {
                AddCallbackRpc(cmd);
            }

            Global.NetManager.Send(peer, packet); 
        }

        public void RpcCallback(ulong protoId, uint protoCode, ulong fromHostId, ulong toHostId, ulong fromActorId, ulong toActorId, NetworkType netType, IMessage cbMsg)
        {  
            var packet = Packet.Create(protoId, protoCode, fromHostId, toHostId, fromActorId, toActorId, netType, cbMsg.GetType(), RpcUtil.Serialize(cbMsg));

            //如果是同进程，则本地调用
            if (fromHostId == toHostId)
            {
                var toActor = Global.Host.GetActor(toActorId);
                toActor.CallMethod(packet);
                return;
            }

            //否则通过网络调用
            var peer = Global.NetManager.GetPeerById(toHostId, netType);

            if (peer == null)
            {
                Log.Warn(string.Format("RpcCallback:cannot_find_peer_and_create {0} => {1} ({2}", fromHostId, toHostId, netType));
                peer = Global.NetManager.CreatePeer(toHostId, null, netType);
            }

            if (peer == null || !peer.IsActive)
            {
                Log.Error(string.Format("RpcCallback:peer disconnected {0}", toHostId));
                //这里可以尝试把global以及redis状态清空
                if(peer == null)
                    Global.NetManager.RemovePeerId(toHostId);
                else
                    Global.NetManager.Deregister(peer);
                return;
            }

            //Log.Info(string.Format("{0} {1} {2} {3} {4}", Global.Host.Id, toHostId,
            //    Global.TypeManager.GetActorType(fromActorId).Name, Global.TypeManager.GetActorType(toActorId).Name,
            //    peer == null ? "NULL" : ""));

            Global.NetManager.Send(peer, packet);
        }

        public ulong AddTimer(long delay, long interval, Action tickCallback)
        {
            //实现timer
            var timer = Timer.Create(delay, interval, false, tickCallback);
            if(this.mTimerDic.TryAdd(timer.Tid, timer))
                return timer.Tid;
            return 0;
        }

        public ulong AddRepeatedTimer(long delay, long interval, Action tickCallback)
        {
            //实现timer
            var timer = Timer.Create(delay, interval, true, tickCallback);
            if (this.mTimerDic.TryAdd(timer.Tid, timer))
                return timer.Tid;
            return 0;
        }

        public bool CancelTimer(ulong timerId)
        {
            return this.mTimerDic.TryRemove(timerId, out var _);
        }

        protected void CheckRpc()
        {
            var curTime = TimeUtil.GetTimeStampMS();
            foreach (var cmd in rpcDic.Values.ToArray())
            {
                if(curTime - cmd.CallTime > 15000)
                {
                    Log.Info("CheckRpc->timeout", cmd.ProtoCode);
                    if (!IsAlive)
                        return;
                    cmd.Callback(null);
                    RemoveRpc(cmd.Id);
                }
            }
        }

        protected void CheckTimer()
        {
            var curTime = TimeUtil.GetTimeStampMS();

            var keys = this.mTimerDic.Keys;

            foreach (var key in keys)
            {
                if (this.mTimerDic.TryGetValue(key, out var t))
                {
                    if (t.CheckTimeout(curTime))
                    {
                        this.mTimerDic.TryRemove(key, out var _);
                        t.Dispose();
                    }
                }
            }
        }

        public virtual void Destroy()
        {
            IsAlive = false;
            foreach (var t in mTimerDic.Values)
                t.Dispose();
            this.mTimerDic.Clear();
        }

        public abstract void Update();

        protected void EntityUpdate()
        {
            if (!IsAlive)
                return;
            CheckTimer();
            CheckRpc();
        }
    }
}
