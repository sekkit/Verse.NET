//

using System.Collections.Concurrent;
using System.Collections.Generic;
using Fenix.Common;
#if !CLIENT
using Fenix.Redis;
using Server.Config.Db;
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

#if !CLIENT
        //protected ConcurrentDictionary<string, RedisDb> redisDic = new ConcurrentDictionary<string, RedisDb>();
 
        protected IdManager()
        {
            Global.DbManager.LoadDb(CacheConfig.HID2ADDR_cache);
            Global.DbManager.LoadDb(CacheConfig.ADDR2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.HID2AID_cache);
            Global.DbManager.LoadDb(CacheConfig.ANAME2AID_cache);
            Global.DbManager.LoadDb(CacheConfig.HNAME2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2ANAME_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2TNAME_cache);
 
            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
        }

        ~IdManager()
        { 
            //foreach (var kv in redisDic)
            //    kv.Value.Dispose();
        }

        //RedisDb GetDb(string key)
        //{
        //    RedisDb db;
        //    redisDic.TryGetValue(key, out db);
        //    return db;
        //}

        string GetKey(string prefix, string key)
        {
            return prefix + ":" + key;
        }
#else
        protected IdManager()
        {
            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
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
            bool ret = Global.DbManager.GetDb(CacheConfig.ADDR2HID).Set(address, host.Id);
            ret = Global.DbManager.GetDb(CacheConfig.HNAME2HID).Set(host.UniqueName, host.Id);
            return Global.DbManager.GetDb(CacheConfig.HID2ADDR).Set(key, address) && ret; 
#else
            return true;
#endif
        }

        public bool RegisterAddress(uint hostId, string address)
        {
            mADDR2HID[address] = hostId;
#if !CLIENT
            var key = hostId.ToString();
            return Global.DbManager.GetDb(CacheConfig.ADDR2HID).Set(address, hostId);
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
            return Global.DbManager.GetDb(CacheConfig.HID2ADDR).Get(key);
#else
            return null;
#endif
        }

        public uint GetHostId(string addr)
        {
            if (mADDR2HID.ContainsKey(addr))
                return mADDR2HID[addr];
#if !CLIENT
            return Global.DbManager.GetDb(CacheConfig.ADDR2HID).Get<uint>(addr);
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

            Global.DbManager.GetDb(CacheConfig.ANAME2AID).Set(actor.UniqueName, actor.Id);
            Global.DbManager.GetDb(CacheConfig.AID2ANAME).Set(actor.Id.ToString(), actor.UniqueName);
            Global.DbManager.GetDb(CacheConfig.AID2TNAME).Set(actor.Id.ToString(), actor.GetType().Name);
            Global.DbManager.GetDb(CacheConfig.HID2AID).Set(host.Id.ToString(), mHID2AID[host.Id]);
            return Global.DbManager.GetDb(CacheConfig.AID2HID).Set(key, host.Id); 
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
            Global.DbManager.GetDb(CacheConfig.AID2ANAME).Delete(hostId.ToString());
            Global.DbManager.GetDb(CacheConfig.ANAME2AID).Delete(hostId.ToString());
            Global.DbManager.GetDb(CacheConfig.AID2TNAME).Delete(hostId.ToString());
            Global.DbManager.GetDb(CacheConfig.AID2HID).Delete(hostId.ToString());
            Global.DbManager.GetDb(CacheConfig.HID2AID).Set(hostId.ToString(), mHID2AID[hostId]);
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
            if (Global.DbManager.GetDb(CacheConfig.HID2ADDR).Get(hostId.ToString()) != null)
            {
                Global.DbManager.GetDb(CacheConfig.HID2ADDR).Delete(hostId.ToString());
                Global.DbManager.GetDb(CacheConfig.ADDR2HID).Delete(addr);
            }

            aids = Global.DbManager.GetDb(CacheConfig.HID2AID).Get<List<uint>>(hostId.ToString());
            if (aids != null)
            {
                Global.DbManager.GetDb(CacheConfig.HID2AID).Delete(hostId.ToString());
                foreach (var aid in aids)
                {
                    Global.DbManager.GetDb(CacheConfig.AID2ANAME).Delete(aid.ToString());
                    Global.DbManager.GetDb(CacheConfig.AID2HID).Delete(aid.ToString());
                    string tname = Global.DbManager.GetDb(CacheConfig.AID2TNAME).Get(aid.ToString());
                    if (tname != null)
                    {
                        Global.DbManager.GetDb(CacheConfig.AID2TNAME).Delete(aid.ToString());
                        Global.DbManager.GetDb(CacheConfig.ANAME2AID).Delete(tname);
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
            return Global.DbManager.GetDb(CacheConfig.ANAME2AID).Get<uint>(key);
#else
            return 0;
#endif
        }

        public string GetActorName(uint actorId)
        {
            if (mAID2ANAME.ContainsKey(actorId))
                return mAID2ANAME[actorId];
#if !CLIENT
            return Global.DbManager.GetDb(CacheConfig.AID2ANAME).Get(actorId.ToString());
#else
            return null;
#endif
        }

        public string GetActorTypename(uint actorId)
        {
            if (mAID2TNAME.ContainsKey(actorId))
                return mAID2TNAME[actorId];
#if !CLIENT
            return Global.DbManager.GetDb(CacheConfig.AID2TNAME).Get(actorId.ToString());
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
            return Global.DbManager.GetDb(CacheConfig.AID2HID).Get<uint>(key);
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
