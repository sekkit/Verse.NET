using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fenix
{
    public class TypeManager
    { 
        public TypeManager()
        {
        }

        protected ConcurrentDictionary<string, Type> mTypeDic = new ConcurrentDictionary<string, Type>();

        protected ConcurrentDictionary<int, Api> RpcTypeDic = new ConcurrentDictionary<int, Api>();

        protected ConcurrentDictionary<int, Type> mMessageTypeDic = new ConcurrentDictionary<int, Type>();
         
        protected ConcurrentDictionary<Type, string> mRef2ATNameDic = new ConcurrentDictionary<Type, string>();

        protected ConcurrentDictionary<string, Type> mATName2RefTypeDic = new ConcurrentDictionary<string, Type>();
         
        public void RegisterType(string name, Type type)
        {
            Log.Info(string.Format("RegisterType: {0} {1}", name, type.FullName));
            this.mTypeDic[name] = type;
        }

        public void RegisterApi(int code, Api api)
        {
            this.RpcTypeDic[code] = api;
        } 
        public Api GetRpcType(int protoCode)
        {  
            if (RpcTypeDic.TryGetValue(Math.Abs(protoCode), out var api))
                return api;

            return Api.NoneApi;
        }

        public void ScanAssemblies(Assembly[] asmList)
        {
            //扫描一下
            foreach (var asm in asmList) 
                foreach (var t in asm.GetTypes())
                {
                    if (RpcUtil.IsHeritedType(t, "Actor"))
                        RegisterType(t.Name, t);
                } 

            foreach (var asm in asmList)
                foreach (var t in asm.GetTypes())
                {
                    var refTypeAttrs = t.GetCustomAttributes(typeof(RefTypeAttribute));
                    if (refTypeAttrs.Count() > 0)
                    {
                        var rta = (RefTypeAttribute)refTypeAttrs.First();
                        //var rtaType = Global.TypeManager.Get(rta.TypeName);
                        Global.TypeManager.RegisterRefType(t, rta.TypeName);
                    }

                    var msgTypeAttrs = t.GetCustomAttributes(typeof(MessageTypeAttribute));
                    if (msgTypeAttrs.Count() > 0)
                    {
                        var mta = (MessageTypeAttribute)msgTypeAttrs.First();
                        Global.TypeManager.RegisterMessageType(mta.ProtoCode, t);
                    }
                } 
        }  

        public void RegisterRefType(Type refType, string targetTypeName)
        {
            this.mRef2ATNameDic[refType] = targetTypeName;
            this.mATName2RefTypeDic[targetTypeName] = refType;
        }

        public void RegisterMessageType(int protoCode, Type type)
        { 
            mMessageTypeDic[Math.Abs(protoCode)] = type;
        }

        public void RegisterActorType(Actor actor)
        {
            ulong actorId = actor.Id;
            string actorName = actor.UniqueName;
            Type type = actor.GetType(); 
            if(!mTypeDic.ContainsKey(type.Name))
                mTypeDic[type.Name] = type; 
        }

        public Type Get(string typeName)
        {
            if (typeName == null)
                return null;
            mTypeDic.TryGetValue(typeName, out var result);
            return result;
        }

        public Type GetActorType(ulong actorId)
        {
            var tname = Global.IdManager.GetActorTypename(actorId);

            return this.Get(tname);
        }

        public Type GetMessageType(int protcolCode)
        {
            //if(!mMessageTypeDic.ContainsKey(protocolId))
            //{

            //}
            return mMessageTypeDic[Math.Abs(protcolCode)];
        }

        public Type GetActorRefType(string typename)
        { 
            if (this.mATName2RefTypeDic.TryGetValue(typename, out Type t))
                return t;
            return typeof(ActorRef);
        }

        public RefType GetRefType(Type refType)
        {
            if(this.mRef2ATNameDic.TryGetValue(refType, out var refTypeName))
            {
                if (refTypeName.StartsWith("Client."))
                    return RefType.CLIENT;
                return RefType.SERVER;
            }
            return RefType.NONE;
        }
    }
}
