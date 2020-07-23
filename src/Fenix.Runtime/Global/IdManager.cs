//

using System.Collections.Concurrent;
using System.Collections.Generic;
using Fenix.Common;
#if !CLIENT
using Fenix.Redis;
#endif

namespace Fenix
{
    public class IdManager
    {
        public static IdManager Instance = new IdManager();

        protected ConcurrentDictionary<uint, string> mHID2ADDR = new ConcurrentDictionary<uint, string>();
            
        protected ConcurrentDictionary<string, uint> mADDR2HID = new ConcurrentDictionary<string, uint>();
        
        protected ConcurrentDictionary<uint, uint> mAID2HID = new ConcurrentDictionary<uint, uint>();
        
        protected ConcurrentDictionary<uint, List<uint>> mHID2AID = new ConcurrentDictionary<uint, List<uint>>();
        
        protected ConcurrentDictionary<string, uint> mHNAME2HID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<string, uint> mANAME2AID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<uint, string> mAID2ANAME = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<uint, string> mAID2TNAME = new ConcurrentDictionary<uint, string>();

        public const string HID2ADDR  = "HID2ADDR";
        public const string ADDR2HID  = "ADDR2HID";
        public const string AID2HID   = "AID2HID";
        public const string HID2AID   = "HID2AID";
        public const string ANAME2AID = "ANAME2AID";
        public const string HNAME2HID = "HNAME2HID";
        public const string AID2ANAME = "AID2ANAME";
        public const string AID2TNAME = "AID2TNAME";

#if !CLIENT
        protected ConcurrentDictionary<string, RedisDb> redisDic = new ConcurrentDictionary<string, RedisDb>();
 
        protected IdManager()
        { 
            redisDic[HID2ADDR]  = new RedisDb(HID2ADDR, "127.0.0.1", 7382);
            redisDic[ADDR2HID]  = new RedisDb(ADDR2HID, "127.0.0.1", 7382);
            redisDic[AID2HID]   = new RedisDb(AID2HID, "127.0.0.1", 7382);
            redisDic[HID2AID]   = new RedisDb(HID2AID, "127.0.0.1", 7382);
            redisDic[ANAME2AID] = new RedisDb(ANAME2AID, "127.0.0.1", 7382);
            redisDic[HNAME2HID] = new RedisDb(HNAME2HID, "127.0.0.1", 7382);
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

        string GetKey(string prefix, string key)
        {
            return prefix + ":" + key;
        }
#else
        protected IdManager()
        {
            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName);
        }

        ~IdManager()
        {

        }

#endif
        public bool RegisterHost(Host host, string address)
        {
            mHID2ADDR[host.Id] = address;
            mADDR2HID[address] = host.Id;

            mHNAME2HID[host.UniqueName] = host.Id;

#if !CLIENT
            var key = host.Id.ToString(); 
            bool ret = GetDb(ADDR2HID).Set(address, host.Id);
            ret = GetDb(HNAME2HID).Set(host.UniqueName, host.Id);
            return GetDb(HID2ADDR).Set(key, address) && ret; 
#else
            return true;
#endif
        }

        public string GetHostAddr(uint hostId)
        {
            if (mHID2ADDR.ContainsKey(hostId))
                return mHID2ADDR[hostId];
#if !CLIENT
            var key = hostId.ToString();
            return GetDb(HID2ADDR).Get(key);
#else
            return null;
#endif
        }

        public uint GetHostId(string addr)
        {
            if (mADDR2HID.ContainsKey(addr))
                return mADDR2HID[addr];
#if !CLIENT
            return GetDb(ADDR2HID).Get<uint>(addr);
#else
            return 0;
#endif
        }

        public bool RegisterActor(Actor actor, Host host)
        {
            mAID2HID[actor.Id] = host.Id;
            if (!mHID2AID.ContainsKey(host.Id))
                mHID2AID[host.Id] = new List<uint>();
            mHID2AID[host.Id].Add(actor.Id);

            mANAME2AID[actor.UniqueName] = actor.Id;

            mAID2ANAME[actor.Id] = actor.UniqueName;

            mAID2TNAME[actor.Id] = actor.GetType().Name;

#if !CLIENT
            var key = actor.Id.ToString();

            GetDb(ANAME2AID).Set(actor.UniqueName, actor.Id);
            GetDb(AID2ANAME).Set(actor.Id.ToString(), actor.UniqueName);
            GetDb(AID2TNAME).Set(actor.Id.ToString(), actor.GetType().Name);
            GetDb(HID2AID).Set(host.Id.ToString(), mHID2AID[host.Id]);
            return GetDb(AID2HID).Set(key, host.Id); 
#else
            return true;
#endif
        } 

        public void RemoveActorId(uint actorId)
        {
            this.mAID2ANAME.TryRemove(actorId, out var aname);
            this.mANAME2AID.TryRemove(aname, out var _);
            this.mAID2TNAME.TryRemove(actorId, out var _);
            this.mAID2HID.TryRemove(actorId, out var hostId); 
            this.mHID2AID[hostId].Remove(actorId);
#if !CLIENT
            GetDb(AID2ANAME).Delete(hostId.ToString());
            GetDb(ANAME2AID).Delete(hostId.ToString());
            GetDb(AID2TNAME).Delete(hostId.ToString());
            GetDb(AID2HID).Delete(hostId.ToString());
            GetDb(HID2AID).Set(hostId.ToString(), mHID2AID[hostId]);
#else
        
#endif
        }

        public void RemoveHostId(uint hostId)
        {
            if (this.mHID2ADDR.TryRemove(hostId, out var addr)) 
                this.mADDR2HID.TryRemove(addr, out var _);

            if (this.mHID2AID.TryGetValue(hostId, out List<uint> aids))
            {
                this.mHID2AID.TryRemove(hostId, out var _); 
                foreach (var aid in aids)
                {
                    this.mAID2ANAME.TryRemove(aid, out var _);
                    this.mAID2HID.TryRemove(aid, out var _);
                    if (this.mAID2TNAME.TryRemove(aid, out string tname))
                        this.mANAME2AID.TryRemove(tname, out var _);
                }
            }
#if !CLIENT
            if (GetDb(HID2ADDR).Get(hostId.ToString()) != null)
            {
                GetDb(HID2ADDR).Delete(hostId.ToString());
                GetDb(ADDR2HID).Delete(addr);
            }

            aids = GetDb(HID2AID).Get<List<uint>>(hostId.ToString());
            if (aids != null)
            {
                GetDb(HID2AID).Delete(hostId.ToString());
                foreach (var aid in aids)
                {
                    GetDb(AID2ANAME).Delete(aid.ToString());
                    GetDb(AID2HID).Delete(aid.ToString());
                    string tname = GetDb(AID2TNAME).Get(aid.ToString());
                    if (tname != null)
                    {
                        GetDb(AID2TNAME).Delete(aid.ToString());
                        GetDb(ANAME2AID).Delete(tname);
                    }
                }
            } 
#else

#endif
        }

        public uint GetActorId(string name)
        {
            if (mANAME2AID.ContainsKey(name))
                return mANAME2AID[name];
#if !CLIENT
            var key = name;
            return GetDb(ANAME2AID).Get<uint>(key);
#else
            return 0;
#endif
        }

        public string GetActorName(uint actorId)
        {
            if (mAID2ANAME.ContainsKey(actorId))
                return mAID2ANAME[actorId];
#if !CLIENT
            return GetDb(AID2ANAME).Get(actorId.ToString());
#else
            return null;
#endif
        }

        public string GetActorTypename(uint actorId)
        {
            if (mAID2TNAME.ContainsKey(actorId))
                return mAID2TNAME[actorId];
#if !CLIENT
            return GetDb(AID2TNAME).Get(actorId.ToString());
#else
            return null;
#endif
        }

        public uint GetHostIdByActorId(uint actorId)
        {
            if (mAID2HID.ContainsKey(actorId))
                return mAID2HID[actorId];
#if !CLIENT
            var key = actorId.ToString();
            return GetDb(AID2HID).Get<uint>(key);
#else
            return 0;
#endif
        }

        public string GetHostAddrByActorId(uint actorId)
        {
            uint hostId = 0;
            if (mAID2HID.ContainsKey(actorId))
            {
                hostId = mAID2HID[actorId];
                return GetHostAddr(hostId);
            }
            hostId = GetHostIdByActorId(actorId);
            return GetHostAddr(hostId);
        }
    }
}
