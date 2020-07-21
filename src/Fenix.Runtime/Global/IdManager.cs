//
using CSRedis;
using System.Collections.Concurrent;
using System.Collections.Generic; 
using Fenix.Common;
using Fenix.Redis;

namespace Fenix
{
    public class IdManager
    {
        public static IdManager Instance = new IdManager();

        protected ConcurrentDictionary<uint, string> mCID2ADDR = new ConcurrentDictionary<uint, string>();
            
        protected ConcurrentDictionary<string, uint> mADDR2CID = new ConcurrentDictionary<string, uint>();
        
        protected ConcurrentDictionary<uint, uint> mAID2CID = new ConcurrentDictionary<uint, uint>();
        
        protected ConcurrentDictionary<uint, List<uint>> mCID2AID = new ConcurrentDictionary<uint, List<uint>>();
        
        protected ConcurrentDictionary<string, uint> mCNAME2CID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<string, uint> mANAME2AID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<uint, string> mAID2ANAME = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<uint, string> mAID2TNAME = new ConcurrentDictionary<uint, string>();

        public const string CID2ADDR  = "CID2ADDR";
        public const string ADDR2CID  = "ADDR2CID";
        public const string AID2CID   = "AID2CID";
        public const string CID2AID   = "CID2AID";
        public const string ANAME2AID = "ANAME2AID";
        public const string CNAME2CID = "CNAME2CID";
        public const string AID2ANAME = "AID2ANAME";
        public const string AID2TNAME = "AID2TNAME";

        protected ConcurrentDictionary<string, RedisDb> redisDic = new ConcurrentDictionary<string, RedisDb>();
         
        protected IdManager()
        {
            redisDic[CID2ADDR]  = new RedisDb(CID2ADDR, "127.0.0.1", 7382);
            redisDic[ADDR2CID] = new RedisDb(ADDR2CID, "127.0.0.1", 7382);
            redisDic[AID2CID] = new RedisDb(AID2CID, "127.0.0.1", 7382);
            redisDic[CID2AID] = new RedisDb(CID2AID, "127.0.0.1", 7382);
            redisDic[ANAME2AID] = new RedisDb(ANAME2AID, "127.0.0.1", 7382);
            redisDic[CNAME2CID] = new RedisDb(CNAME2CID, "127.0.0.1", 7382);
            redisDic[AID2ANAME] = new RedisDb(AID2ANAME, "127.0.0.1", 7382);
            redisDic[AID2TNAME] = new RedisDb(AID2TNAME, "127.0.0.1", 7382);

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName);
        }

        ~IdManager()
        { 
            foreach (var kv in redisDic)
                kv.Value.Dispose();
        }

        RedisDb GetDb(string key)
        {
            RedisDb db;
            redisDic.TryGetValue(key, out db);
            return db;
        }

        //public async Task<bool> RegisterContainerAsync(uint containerId, string address)
        //{
        //    string key = containerId.ToString();
        //    using (var lk = RedisHelper.Lock(key, 3))
        //    {
        //        return await redisClient.SetAsync(key, address);
        //    }
        //}

        //public void RegisterActorType<T>()
        //{
        //    this.mActor2Type[nameof(T)] = typeof(T);
        //}

        //public void RegisterActorType(Type type)
        //{
        //    this.mActor2Type[type.GetType().Name] = type;
        //}

        string GetKey(string prefix, string key)
        {
            return prefix + ":" + key;
        }

        public bool RegisterContainer(Container container, string address)
        {
            mCID2ADDR[container.Id] = address;
            mADDR2CID[address] = container.Id;

            mCNAME2CID[container.UniqueName] = container.Id;

            var key = container.Id.ToString(); 
            bool ret = GetDb(ADDR2CID).Set(address, container.Id);
            ret = GetDb(CNAME2CID).Set(container.UniqueName, container.Id);
            return GetDb(CID2ADDR).Set(key, address) && ret; 
        }

        public string GetContainerAddr(uint containerId)
        {
            if (mCID2ADDR.ContainsKey(containerId))
                return mCID2ADDR[containerId];
            var key = containerId.ToString();
            return GetDb(CID2ADDR).Get(key);
        }

        public uint GetContainerId(string addr)
        {
            if (mADDR2CID.ContainsKey(addr))
                return mADDR2CID[addr];
            return GetDb(ADDR2CID).Get<uint>(addr);
        }

        public bool RegisterActor(Actor actor, Container container)
        {
            mAID2CID[actor.Id] = container.Id;
            if (!mCID2AID.ContainsKey(container.Id))
                mCID2AID[container.Id] = new List<uint>();
            mCID2AID[container.Id].Add(actor.Id);

            mANAME2AID[actor.UniqueName] = actor.Id;

            mAID2ANAME[actor.Id] = actor.UniqueName;

            mAID2TNAME[actor.Id] = actor.GetType().Name;

            var key = actor.Id.ToString();

            GetDb(ANAME2AID).Set(actor.UniqueName, actor.Id);
            GetDb(AID2ANAME).Set(actor.Id.ToString(), actor.UniqueName);
            GetDb(AID2TNAME).Set(actor.Id.ToString(), actor.GetType().Name);
            GetDb(CID2AID).Set(container.Id.ToString(), mCID2AID[container.Id]);
            return GetDb(AID2CID).Set(key, container.Id); 
        } 

        public void RemoveActorId(uint actorId)
        {
            this.mAID2ANAME.TryRemove(actorId, out var aname);
            this.mANAME2AID.TryRemove(aname, out var _);
            this.mAID2TNAME.TryRemove(actorId, out var _);
            this.mAID2CID.TryRemove(actorId, out var cid); 
            this.mCID2AID[cid].Remove(actorId);

            GetDb(AID2ANAME).Delete(cid.ToString());
            GetDb(ANAME2AID).Delete(cid.ToString());
            GetDb(AID2TNAME).Delete(cid.ToString());
            GetDb(AID2CID).Delete(cid.ToString());
            GetDb(CID2AID).Set(cid.ToString(), mCID2AID[cid]);
        }

        public void RemoveContainerId(uint cid)
        {
            if (this.mCID2ADDR.TryRemove(cid, out var addr)) 
                this.mADDR2CID.TryRemove(addr, out var _);

            if (this.mCID2AID.TryGetValue(cid, out List<uint> aids))
            {
                this.mCID2AID.TryRemove(cid, out var _); 
                foreach (var aid in aids)
                {
                    this.mAID2ANAME.TryRemove(aid, out var _);
                    this.mAID2CID.TryRemove(aid, out var _);
                    if (this.mAID2TNAME.TryRemove(aid, out string tname))
                        this.mANAME2AID.TryRemove(tname, out var _);
                }
            }

            if (GetDb(CID2ADDR).Get(cid.ToString()) != null)
            {
                GetDb(CID2ADDR).Delete(cid.ToString());
                GetDb(ADDR2CID).Delete(addr);
            }

            aids = GetDb(CID2AID).Get<List<uint>>(cid.ToString());
            if (aids != null)
            {
                GetDb(CID2AID).Delete(cid.ToString());
                foreach (var aid in aids)
                {
                    GetDb(AID2ANAME).Delete(aid.ToString());
                    GetDb(AID2CID).Delete(aid.ToString());
                    string tname = GetDb(AID2TNAME).Get(aid.ToString());
                    if (tname != null)
                    {
                        GetDb(AID2TNAME).Delete(aid.ToString());
                        GetDb(ANAME2AID).Delete(tname);
                    }
                }
            } 
        }

        public uint GetActorId(string name)
        {
            if (mANAME2AID.ContainsKey(name))
                return mANAME2AID[name];
            var key = name;
            return GetDb(ANAME2AID).Get<uint>(key);
        }

        public string GetActorName(uint actorId)
        {
            if (mAID2ANAME.ContainsKey(actorId))
                return mAID2ANAME[actorId];
            return GetDb(AID2ANAME).Get(actorId.ToString());
        }

        public string GetActorTypename(uint actorId)
        {
            if (mAID2TNAME.ContainsKey(actorId))
                return mAID2TNAME[actorId];
            return GetDb(AID2TNAME).Get(actorId.ToString());
        }

        public uint GetContainerIdByActorId(uint actorId)
        {
            if (mAID2CID.ContainsKey(actorId))
                return mAID2CID[actorId];
            var key = actorId.ToString();
            return GetDb(AID2CID).Get<uint>(key);
        }

        public string GetContainerAddrByActorId(uint actorId)
        {
            uint containerId = 0;
            if (mAID2CID.ContainsKey(actorId))
            {
                containerId = mAID2CID[actorId];
                return GetContainerAddr(containerId);
            }
            containerId = GetContainerIdByActorId(actorId);
            return GetContainerAddr(containerId);
        }
    }
}
