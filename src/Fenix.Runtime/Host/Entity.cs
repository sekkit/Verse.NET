 
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
    public abstract class Entity: IMessage
    { 
        public ulong Id { get; set; }
         
        public string UniqueName { get; set; }
         
        public bool IsAlive { get; set; } = true;
         
        public ConcurrentDictionary<UInt64, RpcCommand> rpcDic     = new ConcurrentDictionary<UInt64, RpcCommand>();

        public ConcurrentDictionary<ulong, long> rpcTimeoutDic = new ConcurrentDictionary<ulong, long>();
         
        public ConcurrentDictionary<int, MethodInfo> rpcStubDic = new ConcurrentDictionary<int, MethodInfo>();
         
        public ConcurrentDictionary<int, MethodInfo> rpcNativeStubDic = new ConcurrentDictionary<int, MethodInfo>();
         
        private ConcurrentDictionary<UInt64, Timer> mTimerDic = new ConcurrentDictionary<ulong, Timer>();

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
                    int code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ServerApi);
                }

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    string protoCode = (isHost ? NameToProtoCode(method.Name) : NameToProtoCode(ns, tname, method.Name)) + "_" + GetApiMessagePostfix(Api.ServerOnly).ToUpper();
                    int code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ServerOnly); 
                }

                attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                if (attrs.Count() > 0)
                {
                    string protoCode = (isHost ? NameToProtoCode(method.Name) : NameToProtoCode(ns, tname, method.Name)) + "_" + GetApiMessagePostfix(Api.ClientApi).ToUpper();
                    int code = Basic.GenID32FromName(protoCode);
                    Global.TypeManager.RegisterApi(code, Api.ClientApi); 
                }

                attrs = method.GetCustomAttributes(typeof(RpcMethodAttribute));
                if (attrs.Count() > 0)
                {
                    var attr = (RpcMethodAttribute)attrs.First();
                    int code = attr.Code;
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
            bool isCallback = packet.ProtoCode < 0;
            //if(!isCallback)
            //    isCallback = rpcDic.ContainsKey(packet.Id);
            //if (!isCallback)  
            //    isCallback = rpcTimeoutDic.ContainsKey(packet.Id);  

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
                var cmd = RpcCommand.Create(packet, null, this); 
                cmd.Call(()=> {
                    RemoveRpc(cmd.Id);
                });
            }
        }

        public virtual void CallMethodWithParams(int protoCode, object[] args)
        {
            try
            {
                Log.Info("CallMethodWithParams:Code", protoCode, this.UniqueName);
                Log.Info("CallMethodWithParams:Name", rpcNativeStubDic[Math.Abs(protoCode)].Name);
                rpcNativeStubDic[Math.Abs(protoCode)].Invoke(this, args);
            }
            catch(Exception ex)
            {
                Log.Error(this.UniqueName);
                Log.Error(ex.ToString());
            }
        }

        public virtual void CallMethodWithMsg(int protoCode, object[] args)
        {
            try
            {
                Log.Info("CallMethodWithMsg:Code", protoCode, this.UniqueName);
                Log.Info("CallMethodWithMsg:Name", rpcStubDic[Math.Abs(protoCode)].Name);
                rpcStubDic[Math.Abs(protoCode)].Invoke(this, args);
            }
            catch (Exception ex)
            {
                Log.Error(this.UniqueName);
                Log.Error(ex.ToString());
            }
        }

        public void Rpc(int protoCode, ulong fromHostId, ulong fromActorId, ulong toHostId, ulong toActorId, 
            IPEndPoint toPeerAddr, NetworkType netType, IMessage msg, Action<byte[]> cb)
        {
            try
            {
                var packet = Packet.Create(Basic.GenID64(), protoCode, fromHostId, toHostId, fromActorId, toActorId, netType, msg.GetType(), msg.Pack());

                /*创建一个等待回调的rpc_command*/
                var cmd = RpcCommand.Create(
                    packet,
                    (data) => { RemoveRpc(packet.Id); cb?.Invoke(data); },
                    this);

            //如果是同进程，则本地调用
            if (fromHostId == toHostId)
            {
                if(Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    if (msg.HasCallback())
                        AddCallbackRpc(cmd);

                    Global.Host.CallMethod(packet);
                    return;
                }
                else
                {
                    var toActor = Global.Host.GetActor(toActorId);

                    if (msg.HasCallback())
                        AddCallbackRpc(cmd);

                    toActor.CallMethod(packet);
                    return;
                }
            }

                bool isClient = Global.IdManager.IsClientHost(toHostId);

                //否则通过网络调用
                //如果是客户端，则直接用remote netpeer返回包
                //如果是服务端，则用local netpeer返回包
                var peer = isClient ? Global.NetManager.GetRemotePeerById(toHostId, netType) : Global.NetManager.GetLocalPeerById(toHostId, netType);
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
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void RpcCallback(ulong protoId, int protoCode, ulong fromHostId, ulong toHostId, ulong fromActorId, ulong toActorId, NetworkType netType, IMessage cbMsg)
        {
            try
            {
                var packet = Packet.Create(protoId, -Math.Abs(protoCode), fromHostId, toHostId, fromActorId, toActorId, netType, cbMsg.GetType(), RpcUtil.Serialize(cbMsg));

            //如果是同进程，则本地调用
            if (fromHostId == toHostId)
            {
                if (Math.Abs(protoCode) < OpCode.CALL_ACTOR_METHOD)
                {
                    Global.Host.CallMethod(packet);
                    return;
                }
                else
                {
                    var toActor = Global.Host.GetActor(toActorId);
                    toActor.CallMethod(packet);
                    return;
                }
            }

                //如果是客户端，则直接用remote netpeer返回包
                //如果是服务端，则用local netpeer返回包
                bool isClient = Global.IdManager.IsClientHost(toHostId);

                //否则通过网络调用
                var peer = isClient ? Global.NetManager.GetRemotePeerById(toHostId, netType) : Global.NetManager.GetLocalPeerById(toHostId, netType);

                if (peer == null)
                {
                    Log.Warn(string.Format("RpcCallback:cannot_find_peer_and_create {0} => {1} ({2}", fromHostId, toHostId, netType));
                    peer = Global.NetManager.CreatePeer(toHostId, null, netType);
                }

                if (peer == null || !peer.IsActive)
                {
                    Log.Error(string.Format("RpcCallback:peer disconnected {0}", toHostId));
                    //这里可以尝试把global以及redis状态清空
                    if (peer == null)
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
            catch(Exception ex)
            {
                Log.Error(ex);
            }
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

        public bool ResetTimer(ulong timerId, long resetInterval)
        {
            if(this.mTimerDic.TryGetValue(timerId, out var tmr))
            {
                tmr.Interval = resetInterval;
                return true;
            }
            return false;
        }

        public bool ExtendTimer(ulong timerId, long extendInterval)
        {
            if (this.mTimerDic.TryGetValue(timerId, out var tmr))
            {
                tmr.Interval = extendInterval;
                return true;
            }
            return false;
        }

        protected void CheckRpc()
        {
            var curTime = TimeUtil.GetTimeStampMS();

            foreach (var key in rpcTimeoutDic.Keys.ToArray())
            {
                var ts = rpcTimeoutDic[key];
                if (curTime - ts >= 30000)
                {
                    rpcTimeoutDic.TryRemove(key, out var _);
                }
            }

            foreach (var cmd in rpcDic.Values.ToArray())
            {
                if (curTime - cmd.CallTime > 15000)
                {
                    Log.Info("CheckRpc->timeout", cmd.ProtoCode);
                    if (!IsAlive)
                        return;

                    cmd.Callback(null);

                    rpcTimeoutDic[cmd.Id] = curTime;
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
