
using Fenix.Common;
using Fenix.Common.Utils;
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
        public uint HostId => Host.Instance.Id;

        [Key(2)]
        [DataMember]
        public string UniqueName { get; set; }

        [Key(3)]
        [DataMember]
        public bool CanTransfer { get; set; }

        public ActorRef Client;

        [IgnoreMember]
        [IgnoreDataMember]
        protected Dictionary<Type, object> mPersistentDic = new Dictionary<Type, object>(); 

        public T Get<T>()
        {
            object value;
            mPersistentDic.TryGetValue(typeof(T), out value);
            return (T)value;
        }
        
        protected Actor(string name) : base()
        {
            this.UniqueName = name;
            this.Id = Basic.GenID32FromName(this.UniqueName);

            this.Init();
        }

        public virtual void Init()
        {
            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
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

        public static Actor Create<T>(string name) where T: Actor, new()
        {
            return Actor.Create(typeof(T), name);
        }

        public static Actor Create(Type type, string name)
        {
            return (Actor)Activator.CreateInstance(type, new object[] { name }); 
        }

        public virtual void Update()
        {
            //Log.Info(string.Format("{0}:{1}", this.GetType().Name, rpcDic.Count));
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
            packData["basic"] = RpcUtil.Serialize(this);
            foreach(var kv in this.mPersistentDic) 
                packData[kv.Key.Name] = RpcUtil.Serialize(kv.Value);
        }

        public virtual void Unpack()
        {
            
        }

        protected void Restore()
        {

        }

        public dynamic GetService(string name)
        {
            return Global.GetActorRef(name, this);
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(name, this);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(uid, this);
        }

        public ActorRef GetActorRef(string name)
        {
            return Global.GetActorRef(name, this);
        }

        public T GetActorRef<T>(string name) where T: ActorRef
        {
            return (T)Global.GetActorRef(name, this);
        }
    }
}
