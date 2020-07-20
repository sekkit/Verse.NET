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
        public uint ContainerId => Container.Instance.Id;

        [Key(2)]
        [DataMember]
        public string UniqueName { get; set; }

        [Key(3)]
        [DataMember]
        public bool CanTransfer { get; set; }
        
        [IgnoreMember]
        [IgnoreDataMember]
        protected Dictionary<Type, object> mPersistentDic = new Dictionary<Type, object>();

        //[IgnoreMember]
        //[IgnoreDataMember]
        //private Container _c;

        //[IgnoreMember]
        //[IgnoreDataMember]
        //public Container Container
        //{
        //    get
        //    {
        //        if(_c == null)

        //    }
        //}

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

        //public static Actor Create(string name)
        //{
        //    return new Actor();
        //}

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
