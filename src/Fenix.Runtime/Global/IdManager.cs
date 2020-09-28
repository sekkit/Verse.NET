//
//#define USE_REDIS_IDMANAGER

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fenix.Common;
using Fenix.Common.Rpc;
using Fenix.Common.Utils; 
using MessagePack;
using Newtonsoft.Json;

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
 
    [MessagePackObject(keyAsPropertyName:true)]
    public class IdDataSet : IMessage
    {
        [Key(0)]
        public ConcurrentDictionary<string, string> mHNAME2ADDR = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        //[IgnoreDataMember]
        public ConcurrentDictionary<string, string> mADDR2HNAME = new ConcurrentDictionary<string, string>();
        
        [Key(1)]
        public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> mHNAME2ANAME = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mANAME2HNAME = new ConcurrentDictionary<string, string>();

        [Key(2)]
        public ConcurrentDictionary<string, string> mANAME2TNAME = new ConcurrentDictionary<string, string>();

        [Key(3)]
        public ConcurrentDictionary<ulong, string> mID2NAME = new ConcurrentDictionary<ulong, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, ulong> mNAME2ID = new ConcurrentDictionary<string, ulong>();

        [Key(4)]
        public ConcurrentDictionary<string, string> mCNAME2ADDR = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mADDR2CNAME = new ConcurrentDictionary<string, string>();
 
        [Key(5)]
        public ConcurrentDictionary<string, string> mIP2EXTIP = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mEXTIP2IP = new ConcurrentDictionary<string, string>();

        [Key(7)]
        public ConcurrentDictionary<string, string> mANAME2CNAME = new ConcurrentDictionary<string, string>();
        //客户端一个Host只有一个actor，所以...
        [Key(8)]
        public ConcurrentDictionary<string, string> mCNAME2ANAME = new ConcurrentDictionary<string, string>();

        [IgnoreMember]
        public ConcurrentDictionary<ulong, ulong> mRPCID2EID = new ConcurrentDictionary<ulong, ulong>();

        public void ReInit()
        {
            foreach (var kv in mHNAME2ADDR)
                mADDR2HNAME[kv.Value] = kv.Key;

            foreach (var kv in mHNAME2ANAME)
                foreach(var aName in kv.Value.Keys)
                    mANAME2HNAME[aName] = kv.Key;

            foreach (var kv in mID2NAME)
                mNAME2ID[kv.Value] = kv.Key;

            foreach (var kv in mCNAME2ADDR)
                mADDR2CNAME[kv.Value] = kv.Key;

            foreach (var kv in mIP2EXTIP)
                mEXTIP2IP[kv.Value] = kv.Key;
        }

        public void UpdateFrom(IdDataSet from)
        {
            Log.Info("Before0", from.ToString());
            Log.Info("Before1", this.ToString());
            foreach (var kv in from.mHNAME2ADDR)
                mHNAME2ADDR[kv.Key] = kv.Value;
            foreach (var kv in from.mHNAME2ANAME)
                foreach (var aName in kv.Value.Keys)
                {
                    if (!mHNAME2ANAME.ContainsKey(kv.Key))
                        mHNAME2ANAME[kv.Key] = new ConcurrentDictionary<string, string>();
                    mHNAME2ANAME[kv.Key][aName] = kv.Key;
                }
            foreach (var kv in from.mANAME2TNAME)
                mANAME2TNAME[kv.Key] = kv.Value;
            foreach (var kv in from.mID2NAME)
                mID2NAME[kv.Key] = kv.Value;
            foreach (var kv in from.mCNAME2ADDR)
                mCNAME2ADDR[kv.Key] = kv.Value;
            foreach (var kv in from.mIP2EXTIP)
                mIP2EXTIP[kv.Key] = kv.Value;
            foreach (var kv in from.mANAME2CNAME)
                mANAME2CNAME[kv.Key] = kv.Value;
            foreach (var kv in from.mCNAME2ANAME)
                mCNAME2ANAME[kv.Key] = kv.Value;
            Log.Info("After0", this.ToString());
            ReInit();
            Log.Info("After1", this.ToString());
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<IdDataSet>(this);
        }

        public new static IdDataSet Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<IdDataSet>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<IdDataSet>.CopyTo(obj, this);
        }

        public override string ToString()
        {
            return ToJson();
        }

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class IdManager
    {
        protected volatile IdDataSet IdData = new IdDataSet();

        public IdDataSet GetIdAll()
        {
            return IdData;
        }

#if !CLIENT

#if USE_REDIS_IDMANAGER
        protected RedisDb CacheHNAME2ADDR => Global.DbManager.GetDb(CacheConfig.HNAME2ADDR);
        protected RedisDb CacheCNAME2ADDR => Global.DbManager.GetDb(CacheConfig.CNAME2ADDR);
        protected RedisDb CacheANAME2HNAME => Global.DbManager.GetDb(CacheConfig.ANAME2HNAME);
        protected RedisDb CacheANAME2CNAME => Global.DbManager.GetDb(CacheConfig.ANAME2CNAME);
        protected RedisDb CacheANAME2TNAME => Global.DbManager.GetDb(CacheConfig.ANAME2TNAME);
        protected RedisDb CacheID2NAME => Global.DbManager.GetDb(CacheConfig.ID2NAME);
        protected RedisDb CacheAddr2ExAddr => Global.DbManager.GetDb(CacheConfig.ADDR2EXTADDR);
  
        [IgnoreMember]
        private Thread th; 

#endif

        public IdManager()
        {
#if USE_REDIS_IDMANAGER
            foreach (var cfg in CacheConfig.Instance.CfgDic)
            {
                Global.DbManager.LoadDb(cfg.Value);
            }
#endif

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
#if USE_REDIS_IDMANAGER
            th = new Thread(new ThreadStart(AutoSync));
            th.Start();
#endif
        }

        ~IdManager()
        {
#if USE_REDIS_IDMANAGER
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

#if USE_REDIS_IDMANAGER
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
            foreach(var hName in IdData.mHNAME2ADDR.Keys) 
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
                IdData.mHNAME2ADDR[hostName] = address;
                IdData.mADDR2HNAME[address] = hostName;

                AddNameId(hostName, hostId);

#if USE_REDIS_IDMANAGER
#if !CLIENT
                CacheAddr2ExAddr.SetWithoutLock(address, extAddress);

                return CacheHNAME2ADDR.SetWithoutLock(hostName, address);
#else
                return true;
#endif
#else

                IdData.mIP2EXTIP[Basic.ToIP(address)] = Basic.ToIP(extAddress);
                IdData.mEXTIP2IP[Basic.ToIP(extAddress)] = Basic.ToIP(address);

                if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
                {
#if !CLIENT
                    Global.IdHostRef.AddHostId(hostId, hostName, address, extAddress, (ok, idAll) =>
                    {
                        Log.Info("AddHost to Id.App", ok, hostId, hostName, address, extAddress);
                        //Global.IdHostRef.GetIdAll(hostId, (ok2, idAll) =>
                        //{
                        if (ok)
                        {
                            Log.Error(idAll.ToString());
                            IdData.UpdateFrom(idAll);
                        }
                        else
                        {
                            bool done = false;
                            while (!done)
                            {
                                var task = Global.IdHostRef.AddHostIdAsync(hostId, hostName, address, extAddress);
                                task.Wait();
                                if (task.Result.arg0)
                                {
                                    done = true;
                                    Log.Error(task.Result.arg1.ToString());
                                    IdData.UpdateFrom(task.Result.arg1);
                                }
                                else
                                {
                                    Thread.Sleep(100);
                                }
                            }
                        }
                        //});
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
                return RegisterClientHost(hostId, hostName, address, noReg);
            }
        }

#if USE_REDIS_IDMANAGER
        public bool ReregisterHost(ulong hostId, string address)
        {
            if (IsClientHost(hostId))
            {
                var hostName = GetName(hostId);
                if (hostName == "" || hostName == null)
                    return false;
                IdData.mCNAME2ADDR[hostName] = address;
                IdData.mADDR2CNAME[address] = hostName;


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
                IdData.mHNAME2ADDR[hostName] = address;
                IdData.mADDR2HNAME[address] = hostName;
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


        public bool RemoveClientHost(ulong clientId, bool noReg=false)
        {
            if (IdData.mID2NAME.TryRemove(clientId, out var cName))
                IdData.mNAME2ID.TryRemove(cName, out var _);

            if (IdData.mCNAME2ANAME.TryRemove(cName, out var aName))
                IdData.mANAME2CNAME.TryRemove(aName, out var _);

            if(IdData.mCNAME2ADDR.TryRemove(cName, out var addr))
                IdData.mADDR2CNAME.TryRemove(addr, out var _);

            //if (IdData.mANAME2TNAME.TryRemove(aName, out var _));


#if USE_REDIS_IDMANAGER
#if !CLIENT
            CacheANAME2CNAME.Delete(aName);
            CacheCNAME2ADDR.Delete(cName);
            CacheID2NAME.Delete(clientId.ToString());
            return CacheANAME2TNAME.Delete(aName);
#endif
#else
            if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
            {
#if !CLIENT
                Global.IdHostRef.RemoveClientHostId(Global.Host.Id, clientId, (ok) =>
                {
                    Log.Info("RemoveClientHost to Id.App", ok, Global.Host.Id, clientId);
                });
                return true;
#else
                return true;
#endif
            }
            return true;
#endif
        }

        public bool RegisterClientHost(ulong clientId, string cName, string address, bool noReg=false)
        { 
            AddNameId(cName, clientId);

            IdData.mCNAME2ADDR[cName] = address;
            IdData.mADDR2CNAME[address] = cName;

#if USE_REDIS_IDMANAGER
#if !CLIENT
            return CacheHNAME2ADDR.SetWithoutLock(cName, address);
#else
            return true;
#endif
#else
            if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
            {
#if !CLIENT
                Global.IdHostRef.AddClientHostId(Global.Host.Id, clientId, cName, address, (ok) =>
                {
                    Log.Info("AddClientHost to Id.App", ok, Global.Host.Id, clientId, cName, address);
                });
                return true;
#else
                return true;
#endif
            }
#endif
            return true;
        } 

        public bool RegisterClientActor(ulong actorId, string actorName, ulong clientId, string address=null, bool noReg=false)
        { 
            AddNameId(actorName, actorId);
            var cName = GetHostName(clientId);
            IdData.mANAME2CNAME[actorName] = cName;
            IdData.mCNAME2ANAME[cName] = actorName;
            if (address != null)
                IdData.mCNAME2ADDR[cName] = address;

#if USE_REDIS_IDMANAGER
#if !CLIENT
            CacheANAME2CNAME.SetWithoutLock(actorName, cName);
            return CacheHNAME2ADDR.SetWithoutLock(cName, address);
#else
            return true;
#endif
#else

            if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
            {
#if !CLIENT
                Global.IdHostRef.AddClientActorId(Global.Host.Id, clientId, actorId, actorName, address, (ok) =>
                {
                    Log.Info("AddClientActor to Id.App", ok, Global.Host.Id, clientId, actorId, actorName, address);
                });
                return true;
#else
                return true;
#endif
            }

            return true;
#endif
        }
 
        public bool IsClientHost(ulong hostId)
        {
            var hostName = GetName(hostId);
            if (hostName == null || hostName == "")
                return false;
            if (IdData.mCNAME2ADDR.ContainsKey(hostName))
                return true;

#if USE_REDIS_IDMANAGER
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
                if (IdData.mHNAME2ADDR.ContainsKey(hostName))
                    return IdData.mHNAME2ADDR[hostName];
#if USE_REDIS_IDMANAGER
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
                if (IdData.mCNAME2ADDR.ContainsKey(hostName))
                    return IdData.mCNAME2ADDR[hostName];

#if USE_REDIS_IDMANAGER
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
            IdData.mADDR2HNAME.TryGetValue(addr, out var result);
            return GetId(result);
        }

        ulong GetId(string name)
        {
            if (name == null)
                return 0;

            if(IdData.mNAME2ID.TryGetValue(name, out var result))
                return result;

#if USE_REDIS_IDMANAGER
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
            if(IdData.mID2NAME.TryGetValue(id, out var result))
                return result;
#if USE_REDIS_IDMANAGER
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
                IdData.mID2NAME[newId] = name;
            IdData.mNAME2ID[name] = newId;
            if(id!=null && id.HasValue)
                IdData.mID2NAME[id.Value] = name;
#if USE_REDIS_IDMANAGER
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
            IdData.mNAME2ID.TryRemove(name, out var id);
            IdData.mID2NAME.TryRemove(id, out var _);
#if USE_REDIS_IDMANAGER
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

                IdData.mANAME2HNAME[aName] = hName;
                if (!IdData.mHNAME2ANAME.ContainsKey(hName))
                    IdData.mHNAME2ANAME[hName] = new ConcurrentDictionary<string, string>();
                IdData.mHNAME2ANAME[hName][aName] = hName;

                IdData.mANAME2TNAME[aName] = aTypeName;

#if USE_REDIS_IDMANAGER
#if !CLIENT
                var ret = CacheANAME2TNAME.SetWithoutLock(aName, aTypeName); 
                return CacheANAME2HNAME.SetWithoutLock(aName, hName) && ret;

#else
                return true;
#endif
#else

                if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
                { 

#if !CLIENT
                    Global.IdHostRef.AddActorId(hostId, actorId, actorName, aTypeName, (ok) =>
                    {
                        Log.Info("AddActor to Id.App", ok, hostId, actorId, actorName, aTypeName);
                    });

                    return true;
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
            IdData.mANAME2HNAME.TryRemove(aName, out var hName); 
            if(hName != null)
                IdData.mHNAME2ANAME[hName].TryRemove(aName, out var _);
            IdData.mANAME2TNAME.TryRemove(aName, out var _);

            RemoveNameId(aName);
#if USE_REDIS_IDMANAGER
#if !CLIENT
            var result = CacheANAME2HNAME.Delete(aName);
            return result && CacheANAME2TNAME.Delete(aName);
#else

#endif
#else
            if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
            {
#if !CLIENT
                Global.IdHostRef.RemoveActorId(actorId, (ok) =>
                {
                    Log.Info("RemoveActorId to Id.App", ok, actorId);
                });
                return true;
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
            if (this.IdData.mHNAME2ADDR.TryRemove(hName, out var addr))
                IdData.mADDR2HNAME.TryRemove(addr, out var _);
             
            if(IdData.mHNAME2ANAME.TryRemove(hName, out var aNames))
            { 
                foreach (var aName in aNames.Keys)
                {
                    IdData.mANAME2HNAME.TryRemove(aName, out var _);
                    IdData.mANAME2TNAME.TryRemove(aName, out string tname);
                    RemoveNameId(aName);
                }
            }

            RemoveNameId(hName);
#if USE_REDIS_IDMANAGER
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
            if (!Global.Host.IsIdHost() && !noReg
#if CLIENT
                )
#else
                    && !Global.SingleProcessMode)
#endif
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
            if (IdData.mANAME2TNAME.ContainsKey(aName))
                return IdData.mANAME2TNAME[aName];
#if USE_REDIS_IDMANAGER
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
                if (IdData.mANAME2CNAME.ContainsKey(aName))
                    return GetId(IdData.mANAME2CNAME[aName]);
#if USE_REDIS_IDMANAGER
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
                if (IdData.mANAME2HNAME.ContainsKey(aName))
                    return GetId(IdData.mANAME2HNAME[aName]);
#if USE_REDIS_IDMANAGER
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
            if (IdData.mCNAME2ANAME.TryGetValue(cName, out var aName))
                return GetId(aName);
            return 0;
        }

        public string GetHostAddrByActorId(ulong actorId, bool isClient=false)
        { 
            var hostId = GetHostIdByActorId(actorId, isClient);
            return GetHostAddr(hostId); 
        }
#if USE_REDIS_IDMANAGER
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

        public string GetIntAddress(string addr)
        {
            string ip = Basic.ToIP(addr);

            var result = IdData.mIP2EXTIP.Where(m => m.Value == ip).Select(m => m.Key).FirstOrDefault();
            if (result != null)
                return string.Format("{0}:{1}", result, Basic.ToPort(addr)); ;

            if (IdData.mIP2EXTIP.ContainsKey(ip))
                return addr;

            return addr;
        }
#endif
#else
#if !CLIENT
        public string GetExtAddress(string addr)
        { 
            string ip = Basic.ToIP(addr);
            IdData.mIP2EXTIP.TryGetValue(ip, out var extIP); 
            if (extIP == null || extIP == "")
            {
                if (IdData.mIP2EXTIP.Any(m => m.Value == ip))
                    return addr;
                return addr;
            }

            return string.Format("{0}:{1}", extIP, Basic.ToPort(addr));
        }

        public string GetIntAddress(string addr)
        {
            string ip = Basic.ToIP(addr);

            var result = IdData.mEXTIP2IP.Where(m => m.Key == ip).Select(m=>m.Key).FirstOrDefault();
            if (result != null)
                return string.Format("{0}:{1}", result, Basic.ToPort(addr)); ;

            if (IdData.mIP2EXTIP.ContainsKey(ip))
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
            IdData.mRPCID2EID[rpcId] = actorId;
        }

        public ulong RemoveRpcId(ulong rpcId)
        {
            IdData.mRPCID2EID.TryRemove(rpcId, out var result);
            return result;
        }

        public ulong GetRpcId(ulong rpcId)
        {
            IdData.mRPCID2EID.TryGetValue(rpcId, out var result);
            return result;
        }

#if !CLIENT
        public HostInfo GetHostInfo(ulong hostId, bool isServiceOnly = true)
        {
            var hostInfo = new HostInfo();
            //该host包含的所有service的id
            //该host所有service的address
            //该host所有service的名称
            var aName = GetName(hostId);
            if (aName == null || aName == "")
                return hostInfo;

            List<string> aList = null;
            if (IdData.mHNAME2ANAME.ContainsKey(aName))
            {
                if (IdData.mHNAME2ANAME.TryGetValue(aName, out var a2hDic))
                {
                    aList = a2hDic.Keys.ToList();
                }
                hostInfo.IsClient = false;
            }
            else if (IdData.mCNAME2ANAME.ContainsKey(aName))
            {
                IdData.mCNAME2ANAME.TryGetValue(aName, out var name);
                aList = new List<string>() { name };

                hostInfo.IsClient = true;
            }

            List<string> svcNameList = new List<string>();
            List<string> actorNameList = new List<string>();

            if (aList != null)
            { 
                svcNameList = aList.Distinct().Where(m => m.EndsWith("Service")).ToList(); 
                if(!isServiceOnly)
                    actorNameList = aList.Distinct().Where(m => !m.EndsWith("Service")).ToList();
            } 

            hostInfo.HostId = hostId;
            hostInfo.HostName = Global.IdManager.GetHostName(hostId);
            hostInfo.HostAddr = Global.IdManager.GetHostAddr(hostId);
            hostInfo.HostExtAddr = Global.IdManager.GetExtAddress(Global.IdManager.GetHostAddr(hostId));
            hostInfo.ServiceId2Name = svcNameList.ToDictionary(m => GetId(m), m => m);
            hostInfo.ServiceId2TName = svcNameList.ToDictionary(m => GetId(m), m => GetActorTypename(GetId(m)));
            hostInfo.ActorId2Name = actorNameList.ToDictionary(m => GetId(m), m => m);
            hostInfo.ActorId2TName = actorNameList.ToDictionary(m => GetId(m), m => GetActorTypename(GetId(m)));
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
        public bool RegisterHostInfo(HostInfo hostInfo)
        {
            AddNameId(hostInfo.HostName, hostInfo.HostId);

            IdData.mADDR2HNAME[hostInfo.HostAddr] = hostInfo.HostName;
            if (hostInfo.HostAddr != null && hostInfo.HostAddr != "")
                IdData.mHNAME2ADDR[hostInfo.HostName] = hostInfo.HostAddr;
            else if (hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
                IdData.mHNAME2ADDR[hostInfo.HostName] = hostInfo.HostExtAddr;
#if USE_REDIS_IDMANAGER
#if !CLIENT
            if(hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
                this.CacheAddr2ExAddr.SetWithoutLock(IdData.mHNAME2ADDR[hostInfo.HostName], hostInfo.HostExtAddr);
#endif
#else
            if (hostInfo.HostExtAddr != null && hostInfo.HostExtAddr != "")
            {
                IdData.mIP2EXTIP[Basic.ToIP(hostInfo.HostAddr)] = Basic.ToIP(hostInfo.HostExtAddr);
                IdData.mEXTIP2IP[Basic.ToIP(hostInfo.HostExtAddr)] = Basic.ToIP(hostInfo.HostAddr);
            }

#endif
            foreach (var kv in hostInfo.ServiceId2Name)
            {
                AddNameId(kv.Value, kv.Key);
                IdData.mANAME2HNAME[kv.Value] = hostInfo.HostName;
                if (!IdData.mHNAME2ANAME.ContainsKey(hostInfo.HostName))
                    IdData.mHNAME2ANAME[hostInfo.HostName] = new ConcurrentDictionary<string, string>();
                if (!IdData.mHNAME2ANAME[hostInfo.HostName].ContainsKey(kv.Value))
                    IdData.mHNAME2ANAME[hostInfo.HostName][kv.Value] = hostInfo.HostName;
            }

            foreach (var kv in hostInfo.ServiceId2TName)
            {
                IdData.mANAME2TNAME[GetName(kv.Key)] = kv.Value;
            }

            return true;
        }

        public bool RegisterActorInfo(ActorInfo actorInfo)
        {
            AddNameId(actorInfo.ActorName, actorInfo.ActorId);
            RegisterHostInfo(actorInfo.HostInfo);
            return this.RegisterActor(actorInfo.ActorId, actorInfo.ActorName, actorInfo.ActorTypeName, actorInfo.HostInfo.HostId, actorInfo.HostInfo.IsClient, noReg: true); 
        }

#if !CLIENT

        public void SyncWithCache()
        {
#if USE_REDIS_IDMANAGER
            var hname2addr  = new Dictionary<string, string>();
            var addr2hname  = new Dictionary<string, string>();
            var aname2hname = new Dictionary<string, string>();
            var hname2aname = new Dictionary<string, List<string>>();
            var aname2cname = new Dictionary<string, string>();
            foreach (var key in CacheHNAME2ADDR.Keys())
            {
                var hName = key;
                var addr = CacheHNAME2ADDR.Get(key);
                if (addr == null) 
                    continue; 
                IdData.mHNAME2ADDR[hName] = addr;
                IdData.mADDR2HNAME[addr] = hName;
                AddNameId(hName);
            }

            foreach (var key in CacheCNAME2ADDR.Keys())
            {
                var cName = key;
                var addr = CacheCNAME2ADDR.Get(key);
                if (addr == null)
                    continue;
                IdData.mCNAME2ADDR[cName] = addr;
                IdData.mADDR2CNAME[addr] = cName;
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

                //IdData.mANAME2HNAME[aName] = hName;
                //IdData.mANAME2HNAME[aName] = hName;
                //if (!mHNAME2ANAME.ContainsKey(hName))
                //    mHNAME2ANAME[hName] = new List<string>();
                //if (!IdData.mHNAME2ANAME[hName].Contains(aName))
                //    IdData.mHNAME2ANAME[hName].Add(aName);
            } 

            foreach (var kv in aname2hname)
                IdData.mANAME2HNAME[kv.Key] = kv.Value;

            foreach (var k in IdData.mANAME2HNAME.Keys.ToArray())
                if (!aname2hname.ContainsKey(k))
                    IdData.mANAME2HNAME.TryRemove(k, out var _);

            foreach (var kv in hname2aname)
            {
                if(!this.mHNAME2ANAME.ContainsKey(kv.Key))
                    this.mHNAME2ANAME[kv.Key] = new ConcurrentDictionary<string, string>();
                foreach (var aName in kv.Value.Keys)
                    this.mHNAME2ANAME[kv.Key][aName] = kv.Key;
            }

            foreach (var key in CacheANAME2TNAME.Keys())
            {
                var aName = key;
                var tName = CacheANAME2TNAME.Get(key);
                if (tName == null)
                    continue;
                IdData.mANAME2TNAME[aName] = tName;
            }

            foreach(var key in CacheID2NAME.Keys())
            {
                var id = ulong.Parse(key);
                var name = CacheID2NAME.Get(key);
                if (name == null)
                    continue;
                IdData.mID2NAME[id] = name;
                IdData.mNAME2ID[name] = id;
            }

            foreach(var key in CacheANAME2CNAME.Keys())
            {
                var aName = key;
                var cName = CacheANAME2CNAME.Get(key);
                if (cName == null)
                    continue;
                IdData.mANAME2CNAME[aName] = cName;
                IdData.mCNAME2ANAME[cName] = aName;
            }
#endif
        }

        public async Task SyncWithCacheAsync()
        {
#if USE_REDIS_IDMANAGER
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
                IdData.mHNAME2ADDR[hName] = addr;
                IdData.mADDR2HNAME[addr] = hName;
                AddNameId(hName);
            }

            foreach (var key in await CacheCNAME2ADDR.KeysAsync())
            {
                var cName = key;
                var addr = await CacheCNAME2ADDR.GetAsync(key);
                if (addr == null)
                    continue;
                IdData.mCNAME2ADDR[cName] = addr;
                IdData.mADDR2CNAME[addr] = cName;
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

                //IdData.mANAME2HNAME[aName] = hName;
                //IdData.mANAME2HNAME[aName] = hName;
                //if (!mHNAME2ANAME.ContainsKey(hName))
                //    mHNAME2ANAME[hName] = new List<string>();
                //if (!IdData.mHNAME2ANAME[hName].Contains(aName))
                //    IdData.mHNAME2ANAME[hName].Add(aName);
            }

            foreach(var kv in aname2hname) 
                IdData.mANAME2HNAME[kv.Key] = kv.Value; 

            foreach(var k in IdData.mANAME2HNAME.Keys.ToArray()) 
                if (!aname2hname.ContainsKey(k))
                    IdData.mANAME2HNAME.TryRemove(k, out var _); 

            foreach (var kv in hname2aname)
            {
                if (!this.mHNAME2ANAME.ContainsKey(kv.Key))
                    this.mHNAME2ANAME[kv.Key] = new ConcurrentDictionary<string, string>();
                foreach (var aName in kv.Value.Keys)
                    this.mHNAME2ANAME[kv.Key][aName] = kv.Key;
            }

            foreach (var key in await CacheANAME2TNAME.KeysAsync())
            {
                var aName = key;
                var tName = await CacheANAME2TNAME.GetAsync(key);
                if (tName == null)
                    continue;
                IdData.mANAME2TNAME[aName] = tName;
            }

            foreach (var key in await CacheID2NAME.KeysAsync())
            {
                var id = ulong.Parse(key);
                var name = await CacheID2NAME.GetAsync(key);
                if (name == null)
                    continue;
                IdData.mID2NAME[id] = name;
                IdData.mNAME2ID[name] = id;
            }

            foreach (var key in await CacheANAME2CNAME.KeysAsync())
            {
                var aName = key;
                var cName = await CacheANAME2CNAME.GetAsync(key);
                if (cName == null)
                    continue;
                IdData.mANAME2CNAME[aName] = cName;
                IdData.mCNAME2ANAME[cName] = aName;
            }
#endif
        }

#endif

#region Utils

        public void PrintInfo()
        {
            Log.Info("**********IDMANAGER*********");
            Log.Info(this.IdData.ToString());
            Log.Info("****************************");
        }

#endregion
    }
}
