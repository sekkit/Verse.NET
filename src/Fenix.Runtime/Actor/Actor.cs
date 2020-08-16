
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
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
    public class Actor: Entity
    {
        [IgnoreMember]
        public ulong HostId => Global.Host.Id;
         
        [Key(3)]
        [DataMember]
        public bool CanTransfer { get; set; }

#if !CLIENT
        
        protected ActorRef clientActor;

        public virtual ActorRef Client => clientActor;

#else

        protected ActorRef serverActor;

        public virtual ActorRef Server => serverActor;

#endif

        [IgnoreMember] 
        protected Dictionary<Type, IMessage> mPersistentDic = new Dictionary<Type, IMessage>();

        [IgnoreMember]
        protected Dictionary<Type, object> mRuntimeDic = new Dictionary<Type, object>();


        public T Get<T>() where T: IMessage
        {
            IMessage value;
            mPersistentDic.TryGetValue(typeof(T), out value);
            return (T)value;
        }
        
        public Actor()
        {
        }

        protected Actor(string name) : base()
        {
            this.UniqueName = name;
            this.Id = Basic.GenID64FromName(this.UniqueName);

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
                        mRuntimeDic[attr.dataType] = Activator.CreateInstance(attr.dataType) as IMessage;
                    }
                }
            }

            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attrs = method.GetCustomAttributes(typeof(PersistentDataAttribute));
                if (attrs.Count() > 0)
                {
                    foreach (PersistentDataAttribute attr in attrs)
                    {
                        mPersistentDic[attr.dataType] = Activator.CreateInstance(attr.dataType) as IMessage;
                    }
                }
            }
        }

        ~Actor()
        {
            //
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
            base.EntityUpdate();

            this.onUpdate();
        }

        //public virtual void Pack()
        //{
        //    Dictionary<string, byte[]> packData = new Dictionary<string, byte[]>();
        //    packData["basic"] = RpcUtil.Serialize(this);
        //    foreach(var kv in this.mPersistentDic) 
        //        packData[kv.Key.Name] = RpcUtil.Serialize(kv.Value);
        //}

        //public virtual void Unpack()
        //{
            
        //}

        public new static Actor Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<Actor>(data);
        }

        public void Restore()
        {

        } 

        public override void Destroy()
        {
            //actor api
            this.onDestroy();

            base.Destroy(); 
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, this, Global.Host);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), uid, this, Global.Host);
        }

        public virtual ActorRef ActorRef()
        {
            return Global.GetActorRef(Global.TypeManager.GetActorRefType(this.GetType().Name), this.UniqueName, this, Global.Host);
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

        //[ServerOnly]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public void OnClientEnable()
        { 
            string actorTypeName = this.GetType().Name;

            var refType = Global.TypeManager.GetActorRefType("Client."+actorTypeName);
            if(refType == null)
                refType = Global.TypeManager.GetActorRefType(this.GetType().FullName);

            this.clientActor = GetActorRef(refType, this.UniqueName);
            Log.Info(string.Format("on_client_enable", this.UniqueName, this.UniqueName));

            this.onClientEnable();

            //this.clientActor.OnServerActorEnable(this.UniqueName);
        }

        //[ServerOnly]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public void OnClientDisable()
        {
            //actorName == this.UniqueName
            this.clientActor = null;
            Log.Info(string.Format("on_client_disable", this.UniqueName, this.UniqueName));

            this.onClientDisable();
        }
#else 

        public void OnServerEnable()
        {
            string actorTypeName = this.GetType().Name;

            var refType = Global.TypeManager.GetActorRefType("Server.UModule." + actorTypeName);
            if (refType == null)
                refType = Global.TypeManager.GetActorRefType(this.GetType().FullName);

            this.serverActor = GetActorRef(refType, this.UniqueName);
            Log.Info(string.Format("on_client_enable", this.UniqueName, this.UniqueName));

            this.onServerEnable();
        }

#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnLoad()
        {
            Log.Info(string.Format("on_load", this.UniqueName, this.UniqueName));

            this.onLoad();
        }

        protected virtual void onLoad()
        {

        }

        [IgnoreMember]
        protected ulong destroyTimerId = 0;

#if !CLIENT
        protected virtual void onClientEnable()
        {
            CancelTimer(destroyTimerId);
        }

        protected virtual void onClientDisable()
        {
            AddTimer(0, 15000, Destroy); 
        } 
#else
        protected virtual void onServerEnable()
        {

        }
#endif
        protected virtual void onRestore()
        {

        }

        protected virtual void onUpdate()
        {

        }

        protected virtual void onDestroy()
        {
            
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<Actor>(this);
        }
    }
}
