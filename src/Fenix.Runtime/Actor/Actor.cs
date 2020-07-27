
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;

namespace Fenix
{
    [MessagePackObject]
    [Serializable]
    public class Actor: Entity
    {
        [IgnoreMember]
        [IgnoreDataMember]
        public uint HostId => Global.Host.Id;
         
        [Key(3)]
        [DataMember]
        public bool CanTransfer { get; set; }

#if !CLIENT
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected ActorRef client;
#endif

        [IgnoreMember]
        [IgnoreDataMember]
        protected Dictionary<Type, object> mPersistentDic = new Dictionary<Type, object>();
 
        public T Get<T>()
        {
            object value;
            mPersistentDic.TryGetValue(typeof(T), out value);
            return (T)value;
        }
        
        public Actor()
        {
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

        public sealed override void Update()
        {
            //Log.Info(string.Format("{0}:{1}", this.GetType().Name, rpcDic.Count)); 

            this.onUpdate();
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

        public void Restore()
        {

        } 

        public override void Destroy()
        {
            base.Destroy();

            //actor api
            this.onDestroy();
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, this, Global.Host);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), uid, this, Global.Host);
        }

        public T GetActorRef<T>(string actorName) where T: ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), actorName, this, Global.Host);
        } 

        public ActorRef GetActorRef(Type refType, string actorName)
        {
            return Global.GetActorRef(refType, actorName, this, Global.Host);
        }

        public T GetService<T>() where T : ActorRef
        {
            var tname = typeof(T).Name;
            string actorName = tname.Substring(0, tname.Length - 3);
            return (T)Global.GetActorRef(typeof(T), actorName, this, Global.Host);
        }

        public T GetService<T>(string hostName, string ip, int port) where T : ActorRef
        {
            var refTypeName = typeof(T).Name;
            string name = refTypeName.Substring(0, refTypeName.Length - 3);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            return (T)Global.GetActorRefByAddr(typeof(T), ep, hostName, name, null, Global.Host);
        }
#if !CLIENT
        [ServerOnly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnClientEnable(string actorName)
        { 
            var refType = Global.TypeManager.GetActorRefType("Client.Avatar");
            if(refType == null)
                refType = Global.TypeManager.GetActorRefType(this.GetType().FullName);

            this.client = GetActorRef(refType, this.UniqueName);
            Log.Info(string.Format("on_client_enable", actorName, this.UniqueName));

            this.onClientEnable();
        }

        [ServerOnly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnClientDisable(string actorName)
        {
            //actorName == this.UniqueName 
            this.client = null;
            Log.Info(string.Format("on_client_disable", actorName, this.UniqueName));

            this.onClientDisable();
        }

#endif

        public virtual void onLoad()
        {

        }

        public virtual void onClientEnable()
        {

        }

        public virtual void onClientDisable()
        {

        }

        public virtual void onRestore()
        {

        }

        public virtual void onUpdate()
        {

        }

        public virtual void onDestroy()
        {
            
        }
    }
}
