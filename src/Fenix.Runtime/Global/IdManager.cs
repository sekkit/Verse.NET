//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Fenix.Common;
#if !CLIENT
using Fenix.Redis;
using Server.Config.Db;
#endif

namespace Fenix
{
    /*
     * ID分两种，一种是固定ID，如Avatar的ID，Service的ID, 
     * 其它如Host的ID，一般是固定的，但是也可以变化
     * GlobalManager里面的Redis全局缓存，只存固定的ID
     */
    public class IdManager
    {
        public static IdManager Instance = new IdManager();

        protected ConcurrentDictionary<uint, string> mHID2ADDR = new ConcurrentDictionary<uint, string>();
            
        protected ConcurrentDictionary<string, uint> mADDR2HID = new ConcurrentDictionary<string, uint>();
        
        protected ConcurrentDictionary<uint, uint> mAID2HID = new ConcurrentDictionary<uint, uint>();
        
        protected ConcurrentDictionary<uint, List<uint>> mHID2AID = new ConcurrentDictionary<uint, List<uint>>();
        
        protected ConcurrentDictionary<string, uint> mHNAME2HID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<uint, string> mHID2HNAME = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<string, uint> mANAME2AID = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<uint, string> mAID2ANAME = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<uint, string> mAID2TNAME = new ConcurrentDictionary<uint, string>();

#if !CLIENT

        protected IdManager()
        {
            Global.DbManager.LoadDb(CacheConfig.HID2ADDR_cache);
            Global.DbManager.LoadDb(CacheConfig.ADDR2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.HID2AID_cache);
            Global.DbManager.LoadDb(CacheConfig.ANAME2AID_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2ANAME_cache);
            Global.DbManager.LoadDb(CacheConfig.AID2TNAME_cache);
            Global.DbManager.LoadDb(CacheConfig.HNAME2HID_cache);
            Global.DbManager.LoadDb(CacheConfig.HID2HNAME_cache); 

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
        }

        ~IdManager()
        { 
            
        } 

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
            return RegisterHost(host.Id, host.UniqueName, address);
        }

        public bool RegisterHost(uint hostId, string hostName, string address)
        {
            mHID2ADDR[hostId] = address;
            mADDR2HID[address] = hostId;

            mHNAME2HID[hostName] = hostId;
            mHID2HNAME[hostId] = hostName;

#if !CLIENT
            var key = hostId.ToString();
            bool ret = Global.DbManager.GetDb(CacheConfig.ADDR2HID).Set(address, hostId);
            ret = Global.DbManager.GetDb(CacheConfig.HNAME2HID).Set(hostName, hostId) && ret;
            ret = Global.DbManager.GetDb(CacheConfig.HID2HNAME).Set(hostId.ToString(), hostName) && ret;
            return Global.DbManager.GetDb(CacheConfig.HID2ADDR).Set(key, address) && ret;
#else
            return true;
#endif
        }

#if !CLIENT
        public void RegisterClientHost(uint clientId, string clientName, string addr)
        {
            mHID2ADDR[clientId] = addr;
            mADDR2HID[addr] = clientId;

            mHNAME2HID[clientName] = clientId;
            mHID2HNAME[clientId] = clientName;

            Global.DbManager.GetDb(CacheConfig.HID2ADDR).Set(clientId.ToString(), addr);
            Global.DbManager.GetDb(CacheConfig.HID2ADDR).Set(addr, clientId);

            Global.DbManager.GetDb(CacheConfig.HNAME2HID).Set(clientName, clientId);
            Global.DbManager.GetDb(CacheConfig.HID2HNAME).Set(clientId.ToString(), clientName);
        }

        public void RegisterClientActor(uint actorId, string actorName, uint clientId, string addr)
        { 
            mAID2HID[actorId] = clientId; 
            //mC_HID2AID[clientId] = actorId;
            mHID2ADDR[clientId] = addr; 
            mADDR2HID[addr] = clientId;
            mAID2ANAME[actorId] = actorName;
            
            Global.DbManager.GetDb(CacheConfig.AID2HID).Set(actorId.ToString(), clientId);
            Global.DbManager.GetDb(CacheConfig.HID2ADDR).Set(clientId.ToString(), addr);
            Global.DbManager.GetDb(CacheConfig.HID2AID).Set(addr, clientId);
            Global.DbManager.GetDb(CacheConfig.AID2ANAME).Set(actorId.ToString(), actorName); 
        }

        //public string GetClientActorAddr(string actorName)
        //{
        //    if (mCNAME2ADDR.ContainsKey(actorName))
        //        return mCNAME2ADDR[actorName];
        //    return Global.DbManager.GetDb(CacheConfig.CNAME2ADDR).Get(actorName);
        //}

#endif

        public string GetHostAddr(uint hostId) 
        { 
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

        } 

        public uint GetHostId(string addr) 
        {
            {
                if (mADDR2HID.ContainsKey(addr))
                    return mADDR2HID[addr];
#if !CLIENT
                return Global.DbManager.GetDb(CacheConfig.ADDR2HID).Get<uint>(addr);
#else
                return 0;
#endif
            } 

        }

        public string GetHostName(uint hostId)
        {
            if (mHID2HNAME.ContainsKey(hostId))
                return mHID2HNAME[hostId];
#if !CLIENT
            return Global.DbManager.GetDb(CacheConfig.HID2HNAME).Get<string>(hostId.ToString());
#else
            return "";
#endif
        }

        public bool RegisterActor(Actor actor, uint hostId)
        {
            mAID2HID[actor.Id] = hostId;
            if (!mHID2AID.ContainsKey(hostId))
                mHID2AID[hostId] = new List<uint>();
            if (!mHID2AID.ContainsKey(actor.Id))
                mHID2AID[hostId].Add(actor.Id);

            mANAME2AID[actor.UniqueName] = actor.Id;

            mAID2ANAME[actor.Id] = actor.UniqueName;

            mAID2TNAME[actor.Id] = actor.GetType().Name;

#if !CLIENT
            var key = actor.Id.ToString();

            Global.DbManager.GetDb(CacheConfig.ANAME2AID).Set(actor.UniqueName, actor.Id);
            Global.DbManager.GetDb(CacheConfig.AID2ANAME).Set(actor.Id.ToString(), actor.UniqueName);
            Global.DbManager.GetDb(CacheConfig.AID2TNAME).Set(actor.Id.ToString(), actor.GetType().Name);
            Global.DbManager.GetDb(CacheConfig.HID2AID).Set(hostId.ToString(), mHID2AID[hostId]);
            return Global.DbManager.GetDb(CacheConfig.AID2HID).Set(key, hostId); 
#else
            return true;
#endif
        }

        //public bool RegisterActor(Actor actor, Host host)
        //{
        //    return RegisterActor(actor, host.Id);
        //} 

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

        public uint GetHostIdByActorId(uint actorId)//, bool isClient)
        {
//            if(isClient)
//            {
//                if (mC_AID2HID.ContainsKey(actorId))
//                    return mC_AID2HID[actorId];
//#if !CLIENT
//                var key = actorId.ToString();
//                return Global.DbManager.GetDb(CacheConfig.C_AID2HID).Get<uint>(key);
//#else
//                    return 0;
//#endif
//            }
//            else
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
            
        }

        public string GetHostAddrByActorId(uint actorId)//, bool isClient)
        {
            //uint hostId = 0;
            //if (mAID2HID.ContainsKey(actorId))
            //{
            //    hostId = mAID2HID[actorId];
            //    return GetHostAddr(hostId, isClient);
            //}

            var hostId = GetHostIdByActorId(actorId);//, isClient);
            return GetHostAddr(hostId);//, isClient);
        }

        public HostInfo GetHostInfo(uint hostId)
        {
            var hostInfo = new HostInfo();
            //该host包含的所有service的id
            //该host所有service的address
            //该host所有service的名称

            this.mHID2AID.TryGetValue(hostId, out var actorList);
            List<uint> serviceIdList = new List<uint>();
            if (actorList != null)
                serviceIdList = actorList.Distinct().Where(m => this.GetActorName(m).EndsWith("Service")).ToList();

            hostInfo.HostId = hostId;
            hostInfo.HostName = Global.IdManager.GetHostName(hostId);
            hostInfo.HostAddr = Global.IdManager.GetHostAddr(hostId);//, false);
            hostInfo.ServiceId2Name = serviceIdList.ToDictionary(m => m, m => this.GetActorName(m));
            hostInfo.ServiceId2TName = serviceIdList.ToDictionary(m => m, m => this.GetActorTypename(m));

            return hostInfo;
        }

        public void RegisterHostInfo(HostInfo hostInfo)
        {
            this.mADDR2HID[hostInfo.HostAddr] = hostInfo.HostId;
            this.mHID2ADDR[hostInfo.HostId] = hostInfo.HostAddr;
            this.mHNAME2HID[hostInfo.HostName] = hostInfo.HostId;
            this.mHID2HNAME[hostInfo.HostId] = hostInfo.HostName; 

            foreach (var kv in hostInfo.ServiceId2Name)
            {
                this.mAID2ANAME[kv.Key] = kv.Value;
                this.mANAME2AID[kv.Value] = kv.Key;
                this.mAID2HID[kv.Key] = hostInfo.HostId;
                if (!mHID2AID.ContainsKey(hostInfo.HostId))
                    mHID2AID[hostInfo.HostId] = new List<uint>();
                if(!this.mHID2AID[hostInfo.HostId].Contains(kv.Key))
                    this.mHID2AID[hostInfo.HostId].Add(kv.Key);
            }

            foreach (var kv in hostInfo.ServiceId2TName)
            {
                this.mAID2TNAME[kv.Key] = kv.Value;
            } 
        }
    }
}
