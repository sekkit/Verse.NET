using Fenix.Common.Attributes;
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
        protected TypeManager()
        {
        }

        public static TypeManager Instance = new TypeManager();

        protected ConcurrentDictionary<string, Type> mTypeDic = new ConcurrentDictionary<string, Type>();

        protected ConcurrentDictionary<uint, Type> mMessageTypeDic = new ConcurrentDictionary<uint, Type>();

        protected ConcurrentDictionary<uint, string> mId2TypenameDic = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<Type, Type> mRef2ActorTypeDic = new ConcurrentDictionary<Type, Type>();

        protected ConcurrentDictionary<Type, Type> mActor2RefTypeDic = new ConcurrentDictionary<Type, Type>();

        public void ScanAssemblies(Assembly[] asmList)
        {
            //扫描一下
            foreach(var asm in asmList)
            foreach (var t in asm.GetTypes())
            {
                var refTypeAttrs = t.GetCustomAttributes(typeof(RefTypeAttribute)); //.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//.GetCustomAttributes(typeof(ResponseAttribute));
                if (refTypeAttrs.Count() > 0)
                {
                    var rta = (RefTypeAttribute)refTypeAttrs.First();
                    Global.TypeManager.RegisterRefType(t, rta.Type);
                }

                var msgTypeAttrs = t.GetCustomAttributes(typeof(MessageTypeAttribute)); //.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//.GetCustomAttributes(typeof(ResponseAttribute));
                if (msgTypeAttrs.Count() > 0)
                {
                    var mta = (MessageTypeAttribute)msgTypeAttrs.First();
                    Global.TypeManager.RegisterMessageType(mta.ProtoCode, t);
                }
            }

            //foreach (var t in typeof(Global).Assembly.GetTypes())
            //    foreach (var attr in t.GetCustomAttributes(typeof(RefTypeAttribute))) //.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//.GetCustomAttributes(typeof(ResponseAttribute));
            //    {
            //        Console.WriteLine(t);
            //    }
        } 

        public void RegisterRefType(Type refType, Type targetType)
        {
            this.mRef2ActorTypeDic[refType] = targetType;
            this.mActor2RefTypeDic[targetType] = refType;
        }

        public void Register(string typeName, Type type)
        {
            mTypeDic[typeName] = type;
        }

        public void RegisterMessageType(uint protocolId, Type type)
        {
            mMessageTypeDic[protocolId] = type;
        }

        public void RegisterActorType(uint actorId, Type type)
        {
            mId2TypenameDic[actorId] = type.Name;
            if(!mTypeDic.ContainsKey(type.Name))
                mTypeDic[type.Name] = type;
        }

        public Type Get(string typeName)
        {
            return mTypeDic[typeName];
        }

        public Type GetMessageType(uint protocolId)
        {
            return mMessageTypeDic[protocolId];
        }

        public Type GetRefType(Type type)
        {
            Type t;
            this.mActor2RefTypeDic.TryGetValue(type, out t);
            return t;
        }
    }
}
