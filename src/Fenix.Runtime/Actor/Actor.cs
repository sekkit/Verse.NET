using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Fenix
{
    [MessagePackObject]
    [Serializable]
    public class Actor: RpcModule //, IActor
    {
        [Key(0)]
        [DataMember]
        public uint Id { get; set; }

        [IgnoreMember]
        [IgnoreDataMember]
        public uint ContainerId { get; set; }

        [Key(2)]
        [DataMember]
        public string UniqueName { get; set; }

        [Key(3)]
        [DataMember]
        public bool CanTransfer { get; set; }
        
        [IgnoreMember]
        [IgnoreDataMember]
        protected Dictionary<Type, object> mPersistentDic = new Dictionary<Type, object>();
         
        public T Get<T>()
        {
            object value;
            mPersistentDic.TryGetValue(typeof(T), out value);
            return (T)value;
        }
        
        protected Actor() : base()
        {
            this.Init();
        }

        public void Init()
        {
            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);//.GetCustomAttributes(typeof(ResponseAttribute));
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attrs = method.GetCustomAttributes(typeof(RuntimeDataAttribute));
                if (attrs.Count() > 0)
                {
                    foreach (RuntimeDataAttribute attr in attrs)
                    {
                        mPersistentDic[attr.dataType] = Activator.CreateInstance(attr.dataType);
                    }
                }
            }
        }

        ~Actor()
        {
            
        }

        public static Actor Create()
        {
            return new Actor();
        }

        public static Actor Create<T>() where T: Actor, new()
        {
            return Actor.Create(typeof(T));
        }
        
        public static Actor Create(Type type)
        {
            return (Actor)Activator.CreateInstance(type);
            //Global.GetActorRef<FightService>("11036").DoSomething();
        }

        public virtual void Update()
        {
             
        }

        public virtual void onActive()
        {
             
        }

        public virtual void onDeactive()
        {
             
        }

        public virtual void onDestroy()
        {
             
        }

        public virtual void Pack()
        {
            Dictionary<string, byte[]> packData = new Dictionary<string, byte[]>();
            packData["basic"] = MessagePackSerializer.Serialize(this);
            foreach(var kv in this.mPersistentDic) 
                packData[kv.Key.Name] = MessagePackSerializer.Serialize(kv.Value);
        }

        public virtual void Unpack()
        {
            
        }

        protected void Restore()
        {

        }
    }
}
