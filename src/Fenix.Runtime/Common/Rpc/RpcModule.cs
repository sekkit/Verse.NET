using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using static Fenix.Common.RpcUtil;

namespace Fenix
{
    public class RpcModule
    { 
        public ConcurrentDictionary<UInt64, RpcCommand> rpcDic = new ConcurrentDictionary<UInt64, RpcCommand>();
        public ConcurrentDictionary<UInt32, Api> rpcTypeDic = new ConcurrentDictionary<UInt32, Api>(); 
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
                    uint protocolId = NetUtil.GenID32FromName(method.Name);
                    rpcStubDic[protocolId] = method;
                    rpcTypeDic[protocolId] = Api.ServerApi;
                }

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    uint protocolId = NetUtil.GenID32FromName(method.Name);
                    rpcStubDic[protocolId] = method;
                    rpcTypeDic[protocolId] = Api.ServerOnly;
                }
            }
        }

        public virtual void CallMethod(uint fromPeerId, Message msg)
        { 
            bool isCallback = this.rpcDic.ContainsKey(msg.Id);
            if (isCallback)
            {
                var cmd = this.rpcDic[msg.Id];
                cmd.Callback(msg.Payload);
            }
            else
            {
                var cmd = RpcCommand.Create(fromPeerId, this.rpcTypeDic[msg.ProtocolId], msg, this);
                cmd.Call();
            }
        }

        public virtual void CallLocalMethod(uint protocolId, object param)
        {
            var kv = new SortedDictionary<int, object>();
            foreach (var fi in param.GetType().GetFields())
            {
                var attr = fi.GetCustomAttributes<KeyAttribute>().FirstOrDefault();
                if (attr == null || attr.IntKey == null)
                    continue;
                kv.Add(attr.IntKey.Value, fi.GetValue(param));
            }
            this.rpcStubDic[protocolId].Invoke(this, kv.Values.ToArray());
        }
    }
}
