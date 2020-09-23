//

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        protected ConcurrentDictionary<ulong, string>  mID2NAME     = new ConcurrentDictionary<ulong, string>();
        protected ConcurrentDictionary<string, ulong>  mNAME2ID     = new ConcurrentDictionary<string, ulong>();

        protected ConcurrentDictionary<string, string> mCNAME2ADDR  = new ConcurrentDictionary<string, string>();
        
        protected ConcurrentDictionary<string, string> mADDR2CNAME  = new ConcurrentDictionary<string, string>();
#if USE_REDIS_IDMANGER
        //
#else
        protected ConcurrentDictionary<string, string> mIP2EXTIP = new ConcurrentDictionary<string, string>();
#endif

        protected ConcurrentDictionary<string, string> mANAME2CNAME = new ConcurrentDictionary<string, string>();
        //客户端一个Host只有一个actor，所以...
        protected ConcurrentDictionary<string, string> mCNAME2ANAME = new ConcurrentDictionary<string, string>();

        protected ConcurrentDictionary<ulong, ulong>   mRPCID2EID   = new ConcurrentDictionary<ulong, ulong>();

#if !CLIENT

#if USE_REDIS_IDMANGER
        protected RedisDb CacheHNAME2ADDR => Global.DbManager.GetDb(CacheConfig.HNAME2ADDR);
        protected RedisDb CacheCNAME2ADDR => Global.DbManager.GetDb(CacheConfig.CNAME2ADDR);
        protected RedisDb CacheANAME2HNAME => Global.DbManager.GetDb(CacheConfig.ANAME2HNAME);
        protected RedisDb CacheANAME2CNAME => Global.DbManager.GetDb(CacheConfig.ANAME2CNAME);
        protected RedisDb CacheANAME2TNAME => Global.DbManager.GetDb(CacheConfig.ANAME2TNAME);
        protected RedisDb CacheID2NAME => Global.DbManager.GetDb(CacheConfig.ID2NAME);
        protected RedisDb CacheAddr2ExAddr => Global.DbManager.GetDb(CacheConfig.ADDR2EXTADDR);
  

        private Thread th; 

#endif

        public IdManager()
        {
#if USE_REDIS_IDMANGER
            foreach (var cfg in CacheConfig.Instance.CfgDic)
            {
                Global.DbManager.LoadDb(cfg.Value);
            }
#endif
            /*
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.HNAME2ADDR));
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.ANAME2CNAME));
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.ANAME2HNAME));
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.ANAME2TNAME));
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.ID2NAME));
            Global.DbManager.LoadDb(CacheConfig.Instance.Get(CacheConfig.ADDR2EXTADDR));
            */

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
#if USE_REDIS_IDMANGER
            th = new Thread(new ThreadStart(AutoSync));
            th.Start();
#endif
        }

        ~IdManager()
        {
#if USE_REDIS_IDMANGER
            try
            {
                th?.Abort();
                th = null;
            }
            catch(Exception ex)
            {

            }
#endif
        }

#if USE_REDIS_IDMANGER
        void AutoSync()
        {
            while (true)
            {
                SyncWithCache();
                Thread.Sleep(500);
            }
        }
#endif

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

        public List<ulong> GetHostIdList()
        {
            var result = new List<ulong>();
            foreach(var hName in this.mHNAME2ADDR.Keys) 
                result.Add(GetId(hName)); 
            return result;
        }
 
        public bool RegisterHost(Host host, string address, string extAddress, bool isClient)
        {
            if (Global.Host == null)
                Global.Host = host;
            else if (Global.Host.Id != host.Id)
                Debug.Assert(Global.Host.Id == host.Id);

            return RegisterHost(host.Id, host.UniqueName, address, extAddress, isClient);
        }

        public bool RegisterHost(ulong hostId, string hostName, string address, string extAddress, bool isClient, bool noReg = false)
        {
            if (!isClient)
            {
                mHNAME2ADDR[hostName] = address;
                mADDR2HNAME[address] = hostName;

                AddNameId(hostName, hostId);

#if USE_REDIS_IDMANGER
#if !CLIENT
                CacheAddr2ExAddr.SetWithoutLock(address, extAddress);

                return CacheHNAME2ADDR.SetWithoutLock(hostName, address);
#else
                return true;
#endif
#else

                mIP2EXTIP[Basic.ToIP(address)] = Basic.ToIP(extAddress);

                if (!Global.Host.IsIdHost() && !noReg)
                {
#if !CLIENT
                    Global.IdHostRef.AddHostId(hostId, hostName, address, extAddress, (ok)=> {
                        Log.Info("AddHost to Id.App", ok, hostId, hostName, address, extAddress);
                        Global.IdHostRef.GetIdAll(hostId, (ok2, hostInfoList) =>
                        {
                            foreach(var hInfo in hostInfoList)
                            {
                                RegisterHostInfo(hInfo);
                            }
                        });
                    });
#else
                    return true;
#endif
                }

                 return true;
#endif
            }
            else
            {
                return RegisterClientHost(hostId, hostName, address);
            }
        }

        //public async Task<bool> ReregisterHostAsync(ulong hostId, string address)
        //{ 
        //    return await Task.Run(() => ReregisterHost(hostId, address));
        //}

#if USE_REDIS_IDMANGER
        public bool ReregisterHost(ulong hostId, string address)
        {
            if (IsClientHost(hostId))
            {
                var hostName = GetName(hostId);
                if (hostName == "" || hostName == null)
                    return false;
                mCNAME2ADDR[hostName] = address;
                mADDR2CNAME[address] = hostName;


#if !CLIENT
                //var extAddr = CacheAddr2ExAddr.Get(address);
                //CacheAddr2ExAddr.SetWithoutLock(address, extAddr);
                return CacheCNAME2ADDR.SetWithoutLock(hostName, address);
#else
                return true;
#endif
 
                return true;
            }
            else
            {
                var hostName = GetName(hostId);
                if (hostName == "" || hostName == null)
                    return false;
                mHNAME2ADDR[hostName] = address;
                mADDR2HNAME[address] = hostName;
                //Log.Error("RRRRRRRRRRRRR13", hostName, address);
 
#if !CLIENT
                var extAddr = CacheAddr2ExAddr.Get(address);
                CacheAddr2ExAddr.SetWithoutLock(address, extAddr);
                return CacheHNAME2ADDR.SetWithoutLock(hostName, address);
#else
                return true;
#endif
 
            }
        }
#endif
         

        public void RemoveClientHost(ulong clientId)
        {
            if (mID2NAME.TryRemove(clientId, out var cName))
                mNAME2ID.TryRemove(cName, out var _);

            if (mCNAME2ANAME.TryRemove(cName, out var aName))
                mANAME2CNAME.TryRemove(aName, out var _);

            mCNAME2ADDR.TryRemove(cName, out var addr);
            mANAME2TNAME.TryRemove(aName, out var _);

#if USE_REDIS_IDMANGER
#if !CLIENT
            CacheANAME2CNAME.Delete(aName);
            CacheCNAME2ADDR.Delete(cName);
            CacheID2NAME.Delete(clientId.ToString());
            CacheANAME2TNAME.Delete(aName);
#endif
#endif
        }

        public bool RegisterClientHost(ulong clientId, string cName, string address)
        { 
            AddNameId(cName, clientId);
            
            mCNAME2ADDR[cName] = address;
            mADDR2CNAME[address] = cName;

#if USE_REDIS_IDMANGER
#if !CLIENT
            return CacheHNAME2ADDR.SetWithoutLock(cName, address);
#else
            return true;
#endif
#else
            return true;
#endif
        } 

        public bool RegisterClientActor(ulong actorId, string actorName, ulong clientId, string address=null)
        { 
            AddNameId(actorName, actorId);
            var cName = GetHostName(clientId);
            mANAME2CNAME[actorName] = cName;
            mCNAME2ANAME[cName] = actorName;
            if (address != null) 
                mCNAME2ADDR[cName] = address;

#if USE_REDIS_IDMANGER
#if !CLIENT
            CacheANAME2CNAME.SetWithoutLock(actorName, cName);
            return CacheHNAME2ADDR.SetWithoutLock(cName, address);
#else
            return true;
#endif
#else
            return true;
#endif
        }
 
        public bool IsClientHost(ulong hostId)
        {
            var hostName = GetName(hostId);
            if (hostName == null || hostName == "")
                return false;
            if (mCNAME2ADDR.ContainsKey(hostName))
                return true;
#if USE_REDIS_IDMANGER
#if !CLIENT
            return CacheCNAME2ADDR.HasKey(hostName);
#else
            return false;
#endif
#else
            return false;
#endif
        }

 
        public string GetHostAddr(ulong hostId)
        {
            bool isClient = IsClientHost(hostId);

            var hostName = GetHostName(hostId);
            if (hostName == null)
                return "";
            if (!isClient)
            {
                if (mHNAME2ADDR.ContainsKey(hostName))
                    return mHNAME2ADDR[hostName];
#if USE_REDIS_IDMANGER
#if !CLIENT
                return CacheHNAME2ADDR.Get(hostName);
#else
                return "";
#endif
#else
                return "";
#endif
            }
            else
            {
                if (mCNAME2ADDR.ContainsKey(hostName))
                    return mCNAME2ADDR[hostName];
#if USE_REDIS_IDMANGER
#if !CLIENT
                return CacheCNAME2ADDR.Get(hostName);
#else
                return "";
#endif
#else
                return "";
#endif
            }
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

            if(mNAME2ID.TryGetValue(name, out var result))
                return result;
#if USE_REDIS_IDMANGER
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
#else
            return 0;
#endif
        }

        string GetName(ulong id)
        {
            if(mID2NAME.TryGetValue(id, out var result))
                return result;
#if USE_REDIS_IDMANGER
#if !CLIENT
            return CacheID2NAME.Get(id.ToString());
#else
            return "";
#endif
#else
            return "";
#endif
        }

        void AddNameId(string name, ulong? id=null)
        {
            var newId = Basic.GenID64FromName(name);
            //if(id.HasValue && id != null && newId != id)
            //{
            //    Log.Error("add_name_id_error", name, id, newId);
            //    return;
            //}
            //Log.Error("ADDNAMEID", name, newId, id);
            if(newId != id)
                mID2NAME[newId] = name;
            mNAME2ID[name] = newId;
            if(id!=null && id.HasValue)
                mID2NAME[id.Value] = name;
#if USE_REDIS_IDMANGER
#if !CLIENT
            CacheID2NAME.SetWithoutLock(newId.ToString(), name);
            if(id!=null&&id.HasValue)
                CacheID2NAME.SetWithoutLock(id.Value.ToString(), name);
#endif
#else
            return;
#endif
        }

        void RemoveNameId(string name)
        {
            mNAME2ID.TryRemove(name, out var id);
            mID2NAME.TryRemove(id, out var _);
#if USE_REDIS_IDMANGER
#if !CLIENT
            CacheID2NAME.Delete(id.ToString());
#endif
#else
            return;
#endif
        }

        public bool RegisterActor(Actor actor, ulong hostId, bool isClient)
        { 
            return RegisterActor(actor.Id, actor.UniqueName, actor.GetType().Name, hostId, isClient); 
        }

        public bool RegisterActor(ulong actorId, string actorName, string aTypeName, ulong hostId, bool isClient, bool noReg=false)
        {
            if (!isClient)
            {
                var aName = actorName;
                var hName = GetName(hostId);
                AddNameId(aName, actorId);

                mANAME2HNAME[aName] = hName;
                if (!mHNAME2ANAME.ContainsKey(hName))
                    mHNAME2ANAME[hName] = new List<string>();
                if (!mHNAME2ANAME[hName].Any(m => m == aName))
                    mHNAME2ANAME[hName].Add(aName);

                mANAME2TNAME[aName] = aTypeName;

#if USE_REDIS_IDMANGER
#if !CLIENT
                var ret = CacheANAME2TNAME.SetWithoutLock(aName, aTypeName); 
                return CacheANAME2HNAME.SetWithoutLock(aName, hName) && ret;

#else
                return true;
#endif
#else

                if (!Global.Host.IsIdHost() && !noReg)
                {
#if !CLIENT
                    Global.IdHostRef.AddActorId(hostId, actorId, actorName, aTypeName, (ok) =>
                    {
                        Log.Info("AddActor to Id.App", ok, hostId, actorId, actorName, aTypeName);
                    });
#else
                    return true;
#endif
                }
                
                return true;
#endif
            }
            else
            { 
                return RegisterClientActor(actorId, actorName, hostId);
            }
        } 

        public bool RemoveActorId(ulong actorId, bool noReg = false)
        {
            if (actorId == 0)
                return false;
            var aName = GetName(actorId);
            if (aName == null)
                return false;
            this.mANAME2HNAME.TryRemove(aName, out var hName); 
            if(hName != null)
                this.mHNAME2ANAME[hName].Remove(aName);
            this.mANAME2TNAME.TryRemove(aName, out var _);

            RemoveNameId(aName);
#if USE_REDIS_IDMANGER
#if !CLIENT
            var result = CacheANAME2HNAME.Delete(aName);
            return result && CacheANAME2TNAME.Delete(aName);
#else

#endif
#else
            if (!Global.Host.IsIdHost() && !noReg)
            {
#if !CLIENT
                Global.IdHostRef.RemoveActorId(actorId, (ok) =>
                {
                    Log.Info("RemoveActorId to Id.App", ok, actorId);
                });
#else
                return true;
#endif
            }

            return true;
#endif
        }

        public bool RemoveHostId(ulong hostId, bool noReg = false)
        {
            var hName = GetName(hostId);
            if (hName == null)
                return false;
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
#if USE_REDIS_IDMANGER
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
            return true;
#else

#endif
#else
            if (!Global.Host.IsIdHost() && !noReg)
            {
#if !CLIENT
                Global.IdHostRef.RemoveHostId(hostId, hName, (ok) =>
                {
                    Log.Info("RemoveHostId to Id.App", ok, hostId, hName);
                });
#else
                return true;
#endif
            }

            return true;
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
#if USE_REDIS_IDMANGER
#if !CLIENT
            return CacheANAME2TNAME.Get(aName);
#else
            return "";
#endif
#else
            return "";
#endif
        }

        public ulong GetHostIdByActorId(ulong actorId, bool isClient=false) 
        {
            var aName = GetName(actorId);
            if (aName == null || aName == "")
                return 0;

            if (isClient)
            {
                if (mANAME2CNAME.ContainsKey(aName))
                    return GetId(mANAME2CNAME[aName]);
#if USE_REDIS_IDMANGER
#if !CLIENT
                return GetId(CacheANAME2CNAME.Get(aName));
#else
                return 0;
#endif
#else
                return 0;
#endif
            }
            else
            {
                if (mANAME2HNAME.ContainsKey(aName))
                    return GetId(mANAME2HNAME[aName]);
#if USE_REDIS_IDMANGER
#if !CLIENT
                return GetId(CacheANAME2HNAME.Get(aName));
#else
                return 0;
#endif
#else
                return 0;
#endif
            } 
        }

        public ulong GetClientActorId(ulong clientHostId)
        {
            var cName = GetName(clientHostId);
            if (cName == null)
                return 0;
            if (mCNAME2ANAME.TryGetValue(cName, out var aName))
                return GetId(aName);
            return 0;
        }

        public string GetHostAddrByActorId(ulong actorId, bool isClient=false)
        { 
            var hostId = GetHostIdByActorId(actorId, isClient);
            return GetHostAddr(hostId); 
        }
#if USE_REDIS_IDMANGER
#if !CLIENT
        public string GetExtAddress(string addr)
        {
            Log.Warn("ExtAddr0", addr);
            var extAddr = this.CacheAddr2ExAddr.Get(addr);
            Log.Warn("ExtAddr1", extAddr);
            if (extAddr == null || extAddr == "")
                return addr;
            return extAddr;
        }
#endif
#else
#if !CLIENT
        public string GetExtAddress(string addr)
        { 
            string ip = Basic.ToIP(addr);
            this.mIP2EXTIP.TryGetValue(ip, out var extIP); 
            if (extIP == null || extIP == "")
            {
                if (this.mIP2EXTIP.Any(m => m.Value == ip))
                    return addr;
                return addr;
            }

            return string.Format("{0}:{1}", extIP, Basic.ToPort(addr));
        }

        public string GetIntAddress(string addr)
        {
            string ip = Basic.ToIP(addr);

            var result = this.mIP2EXTIP.Where(m => m.Value == ip).Select(m=>m.Key).FirstOrDefault();
            if (result != null)
                return string.Format("{0}:{1}", result, Basic.ToPort(addr)); ;

            if (mIP2EXTIP.ContainsKey(ip))
                return addr;

            return addr;
        }

        public bool IsSameLocalhost(string addr0, string addr1)
        { 
            string ip0 = Basic.ToIP(addr0);
            string ip1 = Basic.ToIP(addr1);

            if (ip0 == ip1)
                return true;

            if (GetIntAddress(addr0) == GetIntAddress(addr1))
                return true;

            return false;
        }
#endif
#endif

        public void RegisterRpcId(ulong rpcId, ulong actorId)
        {
            this.mRPCID2EID[rpcId] = actorId;
        }

        public ulong RemoveRpcId(ulong rpcId)
        {
            this.mRPCID2EID.TryRemove(rpcId, out var result);
            return result;
        }

        public ulong GetRpcId(ulong rpcId)
        {
            this.mRPCID2EID.TryGetValue(rpcId, out var result);
            return result;
        }

#if !CLIENT
        public HostInfo GetHostInfo(ulong hostId)
        {
            var hostInfo = new HostInfo();
            //该host包含的所有service的id
            //该host所有service的address
            //该host所有service的名称
            var aName = GetName(hostId); 
            if (aName == null || aName == "")
                return hostInfo;

            List<string> aList = null;
            if(this.mHNAME2ANAME.ContainsKey(aName))
            {
                this.mHNAME2ANAME.TryGetValue(aName, out aList); 
                hostInfo.IsClient = false;
            }
            else if(this.mCNAME2ANAME.ContainsKey(aName))
            {
                this.mCNAME2ANAME.TryGetValue(aName, out var name);
                aList = new List<string>(){name};
                hostInfo.IsClient = true;
            }

            List<string> svcNameList = new List<string>();
            if (aList != null)
                svcNameList = aList.Distinct().Where(m => m.EndsWith("Service")).ToList(); 

            hostInfo.HostId = hostId;
            hostInfo.HostName = Global.IdManager.GetHostName(hostId);
            hostInfo.HostAddr = Global.IdManager.GetHostAddr(hostId);
            hostInfo.HostExtAddr = Global.IdManager.GetExtAddress(Global.IdManager.GetHostAddr(hostId));
            hostInfo.ServiceId2Name = svcNameList.ToDictionary(m => GetId(m), m => m);
            hostInfo.ServiceId2TName = svcNameList.ToDictionary(m => GetId(m), m => m);
            Log.Warn("GetHostInfo", Newtonsoft.Json.JsonConvert.SerializeObject(hostInfo));
            return hostInfo;
        }

        public ActorInfo GetActorInfo(ulong actorId)
        {
            var actorInfo = new ActorInfo();
            var actorName = GetName(actorId);
            if (actorName == null || actorName == "")
                return actorInfo;

            var hostId = Global.IdManager.GetHostIdByActorId(actorId);
            if (hostId == 0)
                return actorInfo;

            actorInfo.ActorId = actorId;
            actorInfo.ActorName = actorName;
            actorInfo.ActorTypeName = Global.IdManager.GetActorTypename(actorId);
            actorInfo.HostInfo = Global.IdManager.GetHostInfo(hostId);
            return actorInfo;
        } 
#endif
        public void RegisterHostInfo(HostInfo hostInfo)
        {
            AddNameId(hostInfo.HostName, hostInfo.HostId);
 
            this.mADDR2HNAME[hostInfo.HostAddr] = hostInfo.HostName;
            if (hostInfo.HostAddr != null && hostInfo.HostAddr != "")
                this.mHNAME2ADDR[hostInfo.HostName] = hostInfo.HostAddr;
            else if (hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
                this.mHNAME2ADDR[hostInfo.HostName] = hostInfo.HostExtAddr;
#if USE_REDIS_IDMANGER
#if !CLIENT
            if(hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
                this.CacheAddr2ExAddr.SetWithoutLock(this.mHNAME2ADDR[hostInfo.HostName], hostInfo.HostExtAddr);
#endif
#else
            if (hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
                this.mIP2EXTIP[Basic.ToIP(hostInfo.HostAddr)] = Basic.ToIP(hostInfo.HostExtAddr);
#endif
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

        public void RegisterActorInfo(ActorInfo actorInfo)
        {
            AddNameId(actorInfo.ActorName, actorInfo.ActorId);

            this.RegisterActor(actorInfo.ActorId, actorInfo.ActorName, actorInfo.ActorTypeName, actorInfo.HostInfo.HostId, actorInfo.HostInfo.IsClient, noReg: true); 
        }

#if !CLIENT

        public void SyncWithCache()
        {
            var hname2addr = new Dictionary<string, string>();
            var addr2hname = new Dictionary<string, string>();
            var aname2hname = new Dictionary<string, string>();
            var hname2aname = new Dictionary<string, List<string>>();
            var aname2cname = new Dictionary<string, string>();
#if USE_REDIS_IDMANGER
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

            foreach (var key in CacheCNAME2ADDR.Keys())
            {
                var cName = key;
                var addr = CacheCNAME2ADDR.Get(key);
                if (addr == null)
                    continue;
                this.mCNAME2ADDR[cName] = addr;
                this.mADDR2CNAME[addr] = cName;
                AddNameId(cName);
            }

            foreach (var key in CacheANAME2HNAME.Keys())
            {
                var aName = key;
                AddNameId(aName);
                var hName = CacheANAME2HNAME.Get(key);
                if (hName == null)
                    continue;
                aname2hname[aName] = hName;
                if (!hname2aname.ContainsKey(hName))
                    hname2aname[hName] = new List<string>();
                if (!hname2aname[hName].Contains(aName))
                    hname2aname[hName].Add(aName);

                //this.mANAME2HNAME[aName] = hName;
                //this.mANAME2HNAME[aName] = hName;
                //if (!mHNAME2ANAME.ContainsKey(hName))
                //    mHNAME2ANAME[hName] = new List<string>();
                //if (!this.mHNAME2ANAME[hName].Contains(aName))
                //    this.mHNAME2ANAME[hName].Add(aName);
            } 

            foreach (var kv in aname2hname)
                this.mANAME2HNAME[kv.Key] = kv.Value;

            foreach (var k in this.mANAME2HNAME.Keys.ToArray())
                if (!aname2hname.ContainsKey(k))
                    this.mANAME2HNAME.TryRemove(k, out var _);

            foreach (var kv in hname2aname)
                this.mHNAME2ANAME[kv.Key] = kv.Value;

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
                this.mID2NAME[id] = name;
                this.mNAME2ID[name] = id;
            }

            foreach(var key in CacheANAME2CNAME.Keys())
            {
                var aName = key;
                var cName = CacheANAME2CNAME.Get(key);
                if (cName == null)
                    continue;
                this.mANAME2CNAME[aName] = cName;
                this.mCNAME2ANAME[cName] = aName;
            }
#endif
        }

        public async Task SyncWithCacheAsync()
        {
#if USE_REDIS_IDMANGER
            var hname2addr = new Dictionary<string, string>();
            var addr2hname = new Dictionary<string, string>();
            var aname2hname = new Dictionary<string, string>();
            var hname2aname = new Dictionary<string, List<string>>();
            var aname2cname = new Dictionary<string, string>();

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

            foreach (var key in await CacheCNAME2ADDR.KeysAsync())
            {
                var cName = key;
                var addr = await CacheCNAME2ADDR.GetAsync(key);
                if (addr == null)
                    continue;
                this.mCNAME2ADDR[cName] = addr;
                this.mADDR2CNAME[addr] = cName;
                AddNameId(cName);
            }

            foreach (var key in await CacheANAME2HNAME.KeysAsync())
            {
                var aName = key;
                AddNameId(aName);
                var hName = await CacheANAME2HNAME.GetAsync(key);
                if (hName == null)
                    continue; 
                aname2hname[aName] = hName;
                if (!hname2aname.ContainsKey(hName))
                    hname2aname[hName] = new List<string>();
                if (!hname2aname[hName].Contains(aName))
                    hname2aname[hName].Add(aName);

                //this.mANAME2HNAME[aName] = hName;
                //this.mANAME2HNAME[aName] = hName;
                //if (!mHNAME2ANAME.ContainsKey(hName))
                //    mHNAME2ANAME[hName] = new List<string>();
                //if (!this.mHNAME2ANAME[hName].Contains(aName))
                //    this.mHNAME2ANAME[hName].Add(aName);
            }

            foreach(var kv in aname2hname) 
                this.mANAME2HNAME[kv.Key] = kv.Value; 

            foreach(var k in this.mANAME2HNAME.Keys.ToArray()) 
                if (!aname2hname.ContainsKey(k))
                    this.mANAME2HNAME.TryRemove(k, out var _); 

            foreach(var kv in hname2aname) 
                this.mHNAME2ANAME[kv.Key] = kv.Value;

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
                this.mID2NAME[id] = name;
                this.mNAME2ID[name] = id;
            }

            foreach (var key in await CacheANAME2CNAME.KeysAsync())
            {
                var aName = key;
                var cName = await CacheANAME2CNAME.GetAsync(key);
                if (cName == null)
                    continue;
                this.mANAME2CNAME[aName] = cName;
                this.mCNAME2ANAME[cName] = aName;
            }
#endif
        }

#endif

#region Utils



#endregion
    }
}
