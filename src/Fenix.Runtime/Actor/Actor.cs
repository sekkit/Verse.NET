
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
#if !CLIENT
using Server.Config;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Fenix
{ 
    public class Actor: Entity
    { 
        public ulong HostId => Global.Host.Id;
          
        public bool CanTransfer { get; set; }

#if !CLIENT
        
        protected ActorRef clientActor;

        public virtual ActorRef Client => clientActor;

#else

        protected ActorRef serverActor;

        public virtual ActorRef Server => serverActor;

#endif

        protected Dictionary<Type, Tuple<string, IMessage>> mPersistentDic = new Dictionary<Type, Tuple<string, IMessage>>();
         
        protected Dictionary<Type, object> mRuntimeDic = new Dictionary<Type, object>();

        protected Dictionary<Type, object> mVolatileDic = new Dictionary<Type, object>();

        protected Dictionary<Type, IActor> mModuleDic = new Dictionary<Type, IActor>();

        public T GetRuntime<T>() where T: IMessage
        { 
            if(mRuntimeDic.TryGetValue(typeof(T), out var value))
                return (T)value;
            return default(T);
        }

        public T GetPersist<T>() where T : IMessage
        { 
            if(mPersistentDic.TryGetValue(typeof(T), out var value))
                return (T)value.Item2;
            return default(T);
        }

        public T GetVolatile<T>()
        {
            if (mVolatileDic.TryGetValue(typeof(T), out var value))
                return (T)value;
            return default(T);
        }

        public T GetModule<T>() where T: IActor
        {
            if (mModuleDic.TryGetValue(typeof(T), out var value))
                return (T)value;
            return default(T);
        }

        protected Actor()
        {
        }

        protected Actor(string name) : base()
        { 
            this.InitWithName(name);
        }

        protected virtual void InitWithName(string name)
        {
            try
            {
                this.UniqueName = name;
                this.Id = Basic.GenID64FromName(this.UniqueName);
                Init();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected ulong GenDataKey(Type type)
        {
            return Basic.GenID64FromName(type.FullName);
        }

        public virtual void Init()
        {
            InitData();

            InitModule();
        }
#if !CLIENT
        protected virtual void InitData()
#else
        protected virtual void InitData()
#endif
        {
            var attrs = GetType().GetCustomAttributes(typeof(RuntimeDataAttribute), true);
            if (attrs.Count() > 0)
            {
                bool needSave = false;
                foreach (RuntimeDataAttribute attr in attrs)
                {
#if !CLIENT
                    mRuntimeDic[attr.DataType] = LoadDataFromDb(DbConf.RUNTIME, attr.DataType);
#endif
                    if (!mRuntimeDic.TryGetValue(attr.DataType, out var d) || (d == null))
                    {
                        mRuntimeDic[attr.DataType] = Activator.CreateInstance(attr.DataType) as IMessage;
                        needSave = true;
                    }
                }
#if !CLIENT
                if (needSave)
                    this.SaveRuntime();
#endif
            }

            attrs = GetType().GetCustomAttributes(typeof(PersistentDataAttribute), true);
            if (attrs.Count() > 0)
            {
                bool needSave = false;
                foreach (PersistentDataAttribute attr in attrs)
                {
#if !CLIENT
                    mPersistentDic[attr.DataType] = new Tuple<string, IMessage>(attr.dbName, LoadDataFromDb(attr.dbName, attr.DataType));
#endif

                    if (!mPersistentDic.TryGetValue(attr.DataType, out var d) || (d == null || d.Item2 == null))
                    {
                        mPersistentDic[attr.DataType] = new Tuple<string, IMessage>(attr.dbName, Activator.CreateInstance(attr.DataType) as IMessage);
                        needSave = true;
                    }
                }
#if !CLIENT
                if (needSave)
                    this.SavePersistent();
#endif
            }

            attrs = GetType().GetCustomAttributes(typeof(VolatileDataAttribute), true);
            if (attrs.Count() > 0)
            {
                foreach (VolatileDataAttribute attr in attrs)
                {
                    if (!mVolatileDic.TryGetValue(attr.DataType, out var d) || (d == null))
                        mVolatileDic[attr.DataType] = Activator.CreateInstance(attr.DataType) as IMessage;
                }
            }
        }

        protected virtual void InitModule()
        {
            var attrs = GetType().GetCustomAttributes(typeof(RequireModuleAttribute), true);
            if (attrs.Count() > 0)
            {
                foreach (RequireModuleAttribute attr in attrs)
                {
                    if (!mModuleDic.TryGetValue(attr.ModuleType, out var d) || (d == null))
                        mModuleDic[attr.ModuleType] = (IActor)Activator.CreateInstance(attr.ModuleType, new object[] { this });
                }
            }
        }

#if !CLIENT

        //protected async Task<IMessage> LoadDataFromDb(string dbName, Type type) 
        //{
        //    string dbKey = this.UniqueName;// + ":" + type.FullName;
        //    var db = Global.DbManager.GetDb(dbName);
        //    var value = await db.GetAsync(type, dbKey);
        //    return (IMessage)(value);
        //}

        //protected async Task<bool> SaveDataToDb(string dbName, Type type, object data)
        //{
        //    string dbKey = this.UniqueName;// + ":" + type.FullName;
        //    var db = Global.DbManager.GetDb(dbName);
        //    return await db.SetAsync(dbKey, data);
        //}

        protected IMessage LoadDataFromDb(string dbName, Type type)
        {
            string dbKey = this.UniqueName;// + ":" + type.FullName;
            var db = Global.DbManager.GetDb(dbName);
            var value = db.Get(type, dbKey);
            return (IMessage)(value);
        }

        protected bool SaveDataToDb(string dbName, Type type, object data)
        {
            string dbKey = this.UniqueName;// + ":" + type.FullName;
            var db = Global.DbManager.GetDb(dbName);
            return db.Set(dbKey, data);
        }

        protected void SaveAll()
        {
            SaveRuntime();
            SavePersistent();
        }

        protected virtual void SaveRuntime()
        {
            foreach (var kv in this.mRuntimeDic)
            {
                SaveDataToDb(DbConf.RUNTIME, kv.Key, kv.Value);
            } 
        }

        protected virtual void SavePersistent()
        {
            foreach (var kv in this.mPersistentDic)
            {
                SaveDataToDb(kv.Value.Item1, kv.Key, kv.Value.Item2);
            }
        }

#endif

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
            //var a = (Actor)Activator.CreateInstance(type, new object[] { name });
            var a = (Actor)Activator.CreateInstance(type);
            a.InitWithName(name);
            return a;
        }

        public sealed override void Update()
        {
            //Log.Info(string.Format("{0}:{1}", this.GetType().Name, rpcDic.Count)); 
            base.EntityUpdate();

            this.onUpdate();

            foreach (var m in this.mModuleDic.Values)
                m.onUpdate();
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
        //
        //} 

        public void Restore()
        {
            this.onRestore();

            foreach (var m in this.mModuleDic.Values)
                m.onRestore();
        } 

        public override void Destroy()
        {
            Log.Info("Actor.Destory>", this.Id, this.UniqueName);
            //actor api
            this.onDestroy();
#if !CLIENT
            var clientId = Global.IdManager.GetHostIdByActorId(this.Id, isClient: true);
            if(clientId != 0)
            {
                Global.Host.RemoveClientActor(this.Id, DisconnectReason.SERVER_ACTOR_DESTROY, (code) => { }, null);
            }

            Global.IdManager.RemoveHostId(clientId);
#endif
            Global.IdManager.RemoveActorId(this.Id);
            
            foreach (var m in this.mModuleDic.Values)
                m.onDestory();

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
         
        public void OnClientEnable()
        { 
            string actorTypeName = this.GetType().Name;

            var refType = Global.TypeManager.GetActorRefType("Client."+actorTypeName);
            if(refType == null)
                refType = Global.TypeManager.GetActorRefType(this.GetType().FullName);

            this.clientActor = GetActorRef(refType, this.UniqueName);
            Log.Info(string.Format("on_client_enable", this.UniqueName, this.clientActor));

            this.onClientEnable();
            foreach (var m in this.mModuleDic.Values)
                m.onClientEnable();

            this.clientActor.OnServerActorEnable(this.UniqueName);
        }
         
        public void OnClientDisable()
        {
            //actorName == this.UniqueName
            this.clientActor = null;
            Log.Info(string.Format("on_client_disable", this.UniqueName, this.UniqueName));

            this.onClientDisable();
            foreach (var m in this.mModuleDic.Values)
                m.onClientDisable();
        }
#else

        public void OnServerEnable()
        {
            string actorTypeName = this.GetType().Name;

            var refType = Global.TypeManager.GetActorRefType("Server.UModule." + actorTypeName);
            if (refType == null)
                refType = Global.TypeManager.GetActorRefType(this.GetType().FullName);

            this.serverActor = GetActorRef(refType, this.UniqueName);

            Log.Info("on_server_enable", this.UniqueName, this.serverActor, this.serverActor.FromHostId);

            this.onServerEnable();
        }

#endif
         
        public virtual void Activate()
        {
            IsAlive = true;
            if (destroyTimerId != 0)
                ExtendTimer(destroyTimerId, 15000);
        }

        public void OnLoad()
        {
            Log.Info(string.Format("on_load", this.UniqueName, this.UniqueName));

            this.onLoad();

            foreach (var m in this.mModuleDic.Values)
                m.onLoad();
        }

        protected virtual void onLoad()
        {

        }

        protected ulong destroyTimerId = 0;

#if !CLIENT
        protected virtual void onClientEnable()
        {
            IsAlive = true;
            if (destroyTimerId != 0)
                CancelTimer(destroyTimerId);
        }

        protected virtual void onClientDisable()
        {
            destroyTimerId = AddTimer(0, 15000, Destroy); 
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
            return null;
            //return MessagePackSerializer.Serialize<Actor>(this);
        }
    }
}
