//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fenix.Common;
using Fenix.Common.Utils;

#if !CLIENT
using Fenix.Redis;
using Server.Config; 
#endif

//路由表的ID
//HOST name,address => host的server固定名称地址
//Actor name => actor名称到host之间的id映射

namespace Fenix
{
    /*
     * ID分两种，一种是固定ID，如Avatar的ID，Service的ID, 
     * 其它如Host的ID，一般是固定的，但是也可以变化
     * GlobalManager里面的Redis全局缓存，只存固定的ID
     */
    public class IdManager
    {
        protected ConcurrentDictionary<string, string> mHNAME2ADDR  = new ConcurrentDictionary<string, string>();
        protected ConcurrentDictionary<string, string> mADDR2HNAME  = new ConcurrentDictionary<string, string>();

        protected ConcurrentDictionary<string, string> mANAME2HNAME = new ConcurrentDictionary<string, string>();
        protected ConcurrentDictionary<string, List<string>> mHNAME2ANAME = new ConcurrentDictionary<string, List<string>>();

        protected ConcurrentDictionary<string, string> mANAME2TNAME = new ConcurrentDictionary<string, string>();

        protected ConcurrentDictionary<ulong, string>   mId2Name       = new ConcurrentDictionary<ulong, string>();
        protected ConcurrentDictionary<string, ulong>   mName2Id       = new ConcurrentDictionary<string, ulong>();

        //protected ConcurrentDictionary<string, string> mCNAME2ADDR = new ConcurrentDictionary<string, string>();
        //protected ConcurrentDictionary<string, string> mCADDR2NAME = new ConcurrentDictionary<string, string>();

        protected ConcurrentDictionary<string, string> mANAME2CNAME = new ConcurrentDictionary<string, string>();
        //客户端一个Host只有一个actor，所以...
        protected ConcurrentDictionary<string, string> mCNAME2ANAME = new ConcurrentDictionary<string, string>();

        protected ConcurrentDictionary<ulong, ulong> mRpcId2EntityId = new ConcurrentDictionary<ulong, ulong>();

#if !CLIENT

        protected RedisDb CacheHNAME2ADDR  => Global.DbManager.GetDb(CacheConfig.HNAME2ADDR);
        protected RedisDb CacheANAME2HNAME => Global.DbManager.GetDb(CacheConfig.ANAME2HNAME);
        protected RedisDb CacheANAME2CNAME => Global.DbManager.GetDb(CacheConfig.ANAME2CNAME);
        protected RedisDb CacheANAME2TNAME => Global.DbManager.GetDb(CacheConfig.ANAME2TNAME); 
        protected RedisDb CacheID2NAME     => Global.DbManager.GetDb(CacheConfig.ID2NAME);
        protected RedisDb CacheAddr2ExAddr => Global.DbManager.GetDb(CacheConfig.ADDR2EXTADDR);

        private Thread th;

        public IdManager()
        {
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.HNAME2ADDR));
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.ANAME2CNAME));
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.ANAME2HNAME));
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.ANAME2TNAME));
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.ID2NAME));
            Global.DbManager.LoadDb(CacheConfig.Get(CacheConfig.ADDR2EXTADDR));

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));

            th = new Thread(new ThreadStart(AutoSync));
            th.Start();
        }

        ~IdManager()
        {
            th?.Abort();
        }

        void AutoSync()
        { 
            SyncWithCache();
            Thread.Sleep(100);
        }

#else
        public IdManager()
        {
            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
        }

        ~IdManager()
        {
        }

#endif


        public bool RegisterHost(Host host, string address, string extAddress)
        {
            return RegisterHost(host.Id, host.UniqueName, address, extAddress);
        }

        public bool RegisterHost(ulong hostId, string hostName, string address, string extAddress)
        {
            mHNAME2ADDR[hostName] = address;
            mADDR2HNAME[address] = hostName;

            AddNameId(hostName, hostId);
#if !CLIENT
            CacheAddr2ExAddr.Set(address, extAddress);
 
            return CacheHNAME2ADDR.Set(hostName, address);
#else
            return true;
#endif
        }

        public async Task<bool> ReregisterHostAsync(ulong hostId, string address)
        {
            return await Task.Run(() => ReregisterHost(hostId, address));
        }

        public bool ReregisterHost(ulong hostId, string address)
        {
            var hostName = GetName(hostId);
            if (hostName == "" || hostName == null)
                return false;
            mHNAME2ADDR[hostName] = address;
            mADDR2HNAME[address] = hostName; 

#if !CLIENT
            var extAddr = CacheAddr2ExAddr.Get(address);
            CacheAddr2ExAddr.Set(address, extAddr);
            return CacheHNAME2ADDR.Set(hostName, address);
#else
            return true;
#endif
        }

#if !CLIENT

        public void RemoveClientHost(ulong clientId)
        {
            if (mId2Name.TryRemove(clientId, out var cName))
                mName2Id.TryRemove(cName, out var _);

            if (mCNAME2ANAME.TryRemove(cName, out var aName))
                mANAME2CNAME.TryRemove(aName, out var _);

            mHNAME2ADDR.TryRemove(cName, out var addr);
            mANAME2TNAME.TryRemove(aName, out var _);

            CacheANAME2CNAME.Delete(aName);
            CacheHNAME2ADDR.Delete(cName);
            CacheID2NAME.Delete(clientId.ToString());
            CacheANAME2TNAME.Delete(aName);
        }

        public void RegisterClientHost(ulong clientId, string clientName, string address)
        { 
            AddNameId(clientName, clientId);

            mHNAME2ADDR[clientName] = address;
            mADDR2HNAME[address] = clientName;

            CacheHNAME2ADDR.Set(clientName, address);
        } 

        public void RegisterClientActor(ulong actorId, string actorName, ulong clientId, string address)
        { 
            AddNameId(actorName, actorId);
            var cName = GetHostName(clientId);
            mANAME2CNAME[actorName] = cName;
            mCNAME2ANAME[cName] = actorName;
            mHNAME2ADDR[cName] = address;

            CacheANAME2CNAME.Set(actorName, cName);
            CacheHNAME2ADDR.Set(cName, address);
        }

#endif

        public string GetHostAddr(ulong hostId)
        {
            var hostName = GetHostName(hostId);
            if (hostName == null)
                return "";
            if (mHNAME2ADDR.ContainsKey(hostName))
                return mHNAME2ADDR[hostName];
#if !CLIENT
            return CacheHNAME2ADDR.Get(hostName);
#else
            return "";
#endif
        } 

        public string GetHostName(ulong hostId)
        {
            return GetName(hostId);
        }

        public ulong GetHostId(string addr)
        {
            mADDR2HNAME.TryGetValue(addr, out var result);
            return GetId(result);
        }

        ulong GetId(string name)
        {
            if (name == null)
                return 0;

            if(mName2Id.TryGetValue(name, out var result))
                return result;
#if !CLIENT
            var tempId = Basic.GenID64FromName(name);
            if (CacheID2NAME.HasKey(tempId.ToString()))
            {
                AddNameId(name);
                return tempId;
            }
            return 0;
#else
            return 0;
#endif
        }

        string GetName(ulong id)
        {
            if(mId2Name.TryGetValue(id, out var result))
                return result;
#if !CLIENT
            return CacheID2NAME.Get(id.ToString());
#else
            return "";
#endif
        }

        void AddNameId(string name, ulong? id=null)
        {
            var newId = Basic.GenID64FromName(name);
            if(id.HasValue && id != null && newId != id)
            {
                Log.Error("add_name_id_error", name, id, newId);
                return;
            }
            //Log.Error("ADDNAMEID", name, newId, id);
            mId2Name[newId] = name;
            mName2Id[name] = newId;

#if !CLIENT
            CacheID2NAME.Set(newId.ToString(), name);
#endif
        }

        void RemoveNameId(string name)
        {
            mName2Id.TryRemove(name, out var id);
            mId2Name.TryRemove(id, out var _);

#if !CLIENT
            CacheID2NAME.Delete(id.ToString());
#endif
        }

        public bool RegisterActor(Actor actor, ulong hostId)
        {
            return RegisterActor(actor.Id, actor.UniqueName, actor.GetType().Name, hostId);
        }

        public bool RegisterActor(ulong actorId, string actorName, string aTypeName, ulong hostId)
        {
            var aName = actorName;
            var hName = GetName(hostId);
            AddNameId(aName, actorId);

            mANAME2HNAME[aName] = hName;
            if (!mHNAME2ANAME.ContainsKey(aName))
                mHNAME2ANAME[hName] = new List<string>();
            if (!mHNAME2ANAME.ContainsKey(aName))
                mHNAME2ANAME[hName].Add(aName);
     
            mANAME2TNAME[aName] = aTypeName;

#if !CLIENT
            var ret = CacheANAME2TNAME.Set(aName, aTypeName); 
            return CacheANAME2HNAME.Set(aName, hName) && ret;

#else
            return true;
#endif
        } 

        public void RemoveActorId(ulong actorId)
        {
            var aName = GetName(actorId);  
            this.mANAME2HNAME.TryRemove(aName, out var hName); 
            this.mHNAME2ANAME[hName].Remove(aName);
            this.mANAME2TNAME.TryRemove(aName, out var _);

            RemoveNameId(aName);

#if !CLIENT
            CacheANAME2HNAME.Delete(aName);
            CacheANAME2TNAME.Delete(aName);
#else

#endif
        }

        public void RemoveHostId(ulong hostId)
        {
            var hName = GetName(hostId);
            if (this.mHNAME2ADDR.TryRemove(hName, out var addr))
                this.mADDR2HNAME.TryRemove(addr, out var _);
             
            if(this.mHNAME2ANAME.TryRemove(hName, out var aNames))
            { 
                foreach (var aName in aNames)
                { 
                    mANAME2HNAME.TryRemove(aName, out var _);
                    mANAME2TNAME.TryRemove(aName, out string tname);
                    RemoveNameId(aName);
                }
            }

            RemoveNameId(hName);

#if !CLIENT
            if(CacheHNAME2ADDR.HasKey(hName))
                CacheHNAME2ADDR.Delete(hName);
            if (aNames != null)
            {
                foreach (var aName in aNames)
                {
                    if (CacheANAME2HNAME.HasKey(aName))
                        CacheANAME2HNAME.Delete(aName);
                    if (CacheANAME2TNAME.HasKey(aName))
                        CacheANAME2TNAME.Delete(aName);
                }
            }
#else

#endif
        }

        public ulong GetActorId(string name)
        {
            return GetId(name);
        }

        public string GetActorName(ulong actorId)
        {
            return GetName(actorId);
        }

        public string GetActorTypename(ulong actorId)
        {
            var aName = GetName(actorId);
            if (mANAME2TNAME.ContainsKey(aName))
                return mANAME2TNAME[aName];
#if !CLIENT
            return CacheANAME2TNAME.Get(aName);
#else
            return null;
#endif
        }

        public ulong GetHostIdByActorId(ulong actorId, bool isClient=false) 
        {
            var aName = GetName(actorId);
            if (aName == null)
                return 0;
            if(isClient)
            {
                if (mANAME2CNAME.ContainsKey(aName))
                    return GetId(mANAME2CNAME[aName]);
#if !CLIENT
                return GetId(CacheANAME2CNAME.Get(aName));
#else
            return 0;
#endif
            }
            else
            {
                if (mANAME2HNAME.ContainsKey(aName))
                    return GetId(mANAME2HNAME[aName]);
#if !CLIENT
                return GetId(CacheANAME2HNAME.Get(aName));
#else
            return 0;
#endif
            } 
        }

        public ulong GetClientActorId(ulong clientHostId)
        {
            var cName = GetName(clientHostId);
            if (mCNAME2ANAME.TryGetValue(cName, out var aName))
                return GetId(aName);
            return 0;
        }

        public string GetHostAddrByActorId(ulong actorId, bool isClient=false)
        { 
            var hostId = GetHostIdByActorId(actorId, isClient);
            return GetHostAddr(hostId); 
        }

#if !CLIENT
        public string GetExtAddress(string addr)
        {
            return this.CacheAddr2ExAddr.Get(addr)??"";
        }
#endif

        public void RegisterRpcId(ulong rpcId, ulong actorId)
        {
            this.mRpcId2EntityId[rpcId] = actorId;
        }

        public ulong RemoveRpcId(ulong rpcId)
        {
            this.mRpcId2EntityId.TryRemove(rpcId, out var result);
            return result;
        }

        public ulong GetRpcId(ulong rpcId)
        {
            this.mRpcId2EntityId.TryGetValue(rpcId, out var result);
            return result;
        }

        public HostInfo GetHostInfo(ulong hostId)
        {
            var hostInfo = new HostInfo();
            //该host包含的所有service的id
            //该host所有service的address
            //该host所有service的名称
            var aName = GetName(hostId);
            this.mHNAME2ANAME.TryGetValue(aName, out var aList);
            List<string> svcNameList = new List<string>();
            if (aList != null)
                svcNameList = aList.Distinct().Where(m => m.EndsWith("Service")).ToList();

            hostInfo.HostId = hostId;
            hostInfo.HostName = Global.IdManager.GetHostName(hostId);
            hostInfo.HostAddr = Global.IdManager.GetHostAddr(hostId);//, false);
            hostInfo.ServiceId2Name = svcNameList.ToDictionary(m => GetId(m), m => m);
            hostInfo.ServiceId2TName = svcNameList.ToDictionary(m => GetId(m), m => m);

            return hostInfo;
        }

        public void RegisterHostInfo(HostInfo hostInfo)
        {
            AddNameId(hostInfo.HostName, hostInfo.HostId);

            this.mADDR2HNAME[hostInfo.HostAddr] = hostInfo.HostName;
            this.mHNAME2ADDR[hostInfo.HostName] = hostInfo.HostAddr; 

            foreach (var kv in hostInfo.ServiceId2Name)
            {
                AddNameId(kv.Value, kv.Key); 
                this.mANAME2HNAME[kv.Value] = hostInfo.HostName;
                if (!mHNAME2ANAME.ContainsKey(hostInfo.HostName))
                    mHNAME2ANAME[hostInfo.HostName] = new List<string>();
                if (!this.mHNAME2ANAME[hostInfo.HostName].Contains(kv.Value))
                    this.mHNAME2ANAME[hostInfo.HostName].Add(kv.Value);
            }

            foreach (var kv in hostInfo.ServiceId2TName)
            {
                this.mANAME2TNAME[GetName(kv.Key)] = kv.Value;
            }
        }

#if !CLIENT

        public void SyncWithCache()
        { 
            foreach (var key in CacheHNAME2ADDR.Keys())
            {
                var hName = key;
                var addr = CacheHNAME2ADDR.Get(key);
                if (addr == null) 
                    continue; 
                this.mHNAME2ADDR[hName] = addr;
                this.mADDR2HNAME[addr] = hName;
                AddNameId(hName);
            }

            foreach (var key in CacheANAME2HNAME.Keys())
            { 
                var aName = key;
                AddNameId(aName);
                var hName = CacheANAME2HNAME.Get(key);
                if (hName == null)
                    continue;
                this.mANAME2HNAME[aName] = hName;
                if (!mHNAME2ANAME.ContainsKey(hName))
                    mHNAME2ANAME[hName] = new List<string>();
                if (!this.mHNAME2ANAME[hName].Contains(aName))
                    this.mHNAME2ANAME[hName].Add(aName);  
            }

            foreach (var key in CacheANAME2TNAME.Keys())
            {
                var aName = key;
                var tName = CacheANAME2TNAME.Get(key);
                if (tName == null)
                    continue;
                this.mANAME2TNAME[aName] = tName;
            }

            foreach(var key in CacheID2NAME.Keys())
            {
                var id = ulong.Parse(key);
                var name = CacheID2NAME.Get(key);
                if (name == null)
                    continue;
                this.mId2Name[id] = name;
                this.mName2Id[name] = id;
            }

            foreach(var key in CacheANAME2CNAME.Keys())
            {
                var aName = key;
                var cName = CacheANAME2CNAME.Get(key); 
                this.mANAME2CNAME[aName] = cName;
                this.mCNAME2ANAME[cName] = aName;
            }
        }

        public async Task SyncWithCacheAsync()
        {
            foreach (var key in await CacheHNAME2ADDR.KeysAsync())
            {
                var hName = key;
                var addr = await CacheHNAME2ADDR.GetAsync(key);
                if (addr == null)
                    continue;
                this.mHNAME2ADDR[hName] = addr;
                this.mADDR2HNAME[addr] = hName;
                AddNameId(hName);
            }

            foreach (var key in await CacheANAME2HNAME.KeysAsync())
            {
                var aName = key;
                AddNameId(aName);
                var hName = await CacheANAME2HNAME.GetAsync(key);
                if (hName == null)
                    continue;
                this.mANAME2HNAME[aName] = hName;
                if (!mHNAME2ANAME.ContainsKey(hName))
                    mHNAME2ANAME[hName] = new List<string>();
                if (!this.mHNAME2ANAME[hName].Contains(aName))
                    this.mHNAME2ANAME[hName].Add(aName);
            }

            foreach (var key in await CacheANAME2TNAME.KeysAsync())
            {
                var aName = key;
                var tName = await CacheANAME2TNAME.GetAsync(key);
                if (tName == null)
                    continue;
                this.mANAME2TNAME[aName] = tName;
            }

            foreach (var key in await CacheID2NAME.KeysAsync())
            {
                var id = ulong.Parse(key);
                var name = await CacheID2NAME.GetAsync(key);
                if (name == null)
                    continue;
                this.mId2Name[id] = name;
                this.mName2Id[name] = id;
            }

            foreach (var key in await CacheANAME2CNAME.KeysAsync())
            {
                var aName = key;
                var cName = await CacheANAME2CNAME.GetAsync(key);
                this.mANAME2CNAME[aName] = cName;
                this.mCNAME2ANAME[cName] = aName;
            }
        }

#endif
    }
}
