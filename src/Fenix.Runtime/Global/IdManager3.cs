using Fenix.Common;
using Fenix.Common.Rpc; 
using MessagePack;
using Newtonsoft.Json; 
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Fenix.Common.Utils;

#if !CLIENT
using Fenix.Redis;
using Server.Config;
#endif

namespace Fenix
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RouteDataSet : IMessage
    {
        public ConcurrentDictionary<ulong, HostRouteData> HostRouteDic { get; set; } = new ConcurrentDictionary<ulong, HostRouteData>();

        public ConcurrentDictionary<ulong, HostRouteData> ActorRouteDic { get; set; } = new ConcurrentDictionary<ulong, HostRouteData>();

        /*ROUTE INDEX DATA*/
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mHNAME2ADDR = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mADDR2HNAME = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> mHNAME2ANAME = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mANAME2HNAME = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mANAME2TNAME = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<ulong, string> mID2NAME = new ConcurrentDictionary<ulong, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, ulong> mNAME2ID = new ConcurrentDictionary<string, ulong>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mCNAME2ADDR = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mADDR2CNAME = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mIP2EXTIP = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mEXTIP2IP = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mANAME2CNAME = new ConcurrentDictionary<string, string>();
        //客户端一个Host只有一个actor，所以...
        [IgnoreMember]
        public ConcurrentDictionary<string, string> mCNAME2ANAME = new ConcurrentDictionary<string, string>();
        [IgnoreMember]
        public ConcurrentDictionary<ulong, ulong> mRPCID2EID = new ConcurrentDictionary<ulong, ulong>();
        //[IgnoreMember]
        //public ConcurrentDictionary<ulong, ulong> mADDRID2ID = new ConcurrentDictionary<ulong, ulong>();
        //[IgnoreMember]
        //public ConcurrentDictionary<ulong, ulong> mID2ADDRID = new ConcurrentDictionary<ulong, ulong>();

        public void ReInit()
        {
            foreach (var kv in mHNAME2ADDR)
                mADDR2HNAME[kv.Value] = kv.Key;

            foreach (var kv in mHNAME2ANAME)
                foreach (var aName in kv.Value.Keys)
                    mANAME2HNAME[aName] = kv.Key;

            foreach (var kv in mID2NAME)
                mNAME2ID[kv.Value] = kv.Key;

            mADDR2CNAME.Clear();
            foreach (var kv in mCNAME2ADDR)
                mADDR2CNAME[kv.Value] = kv.Key;

            foreach (var kv in mIP2EXTIP)
                mEXTIP2IP[kv.Value] = kv.Key;
        }

        public void UpdateFrom(RouteDataSet from)
        {
            Log.Info("Before0", from.ToString());
            Log.Info("Before1", this.ToString());

            foreach (var kv in from.mHNAME2ADDR)
                mHNAME2ADDR[kv.Key] = kv.Value;

            foreach (var kv in from.mHNAME2ANAME)
            {
                foreach (var aName in kv.Value.Keys.ToArray())
                {
                    if (!mHNAME2ANAME.ContainsKey(kv.Key))
                        mHNAME2ANAME[kv.Key] = new ConcurrentDictionary<string, string>();
                    mHNAME2ANAME[kv.Key].TryAdd(aName, kv.Key);
                }
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
                if (kv.Key != "" && kv.Value != "" && kv.Key != null && kv.Value != null)
                    mANAME2CNAME[kv.Key] = kv.Value;
            foreach (var kv in from.mCNAME2ANAME)
                if (kv.Key != "" && kv.Value != "" && kv.Key != null && kv.Value != null)
                    mCNAME2ANAME[kv.Key] = kv.Value;

            Log.Info("After0", this.ToString());
            ReInit();
            Log.Info("After1", this.ToString());
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<RouteDataSet>(this);
        }

        public new static RouteDataSet Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<RouteDataSet>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<RouteDataSet>.CopyTo(obj, this);
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
        protected volatile RouteDataSet mRouteData = new RouteDataSet();

#if !CLIENT
        protected RedisDb CacheHNAME2ADDR { get; set; }
        protected RedisDb CacheCNAME2ADDR { get; set; }
        protected RedisDb CacheANAME2HNAME { get; set; }
        protected RedisDb CacheANAME2CNAME { get; set; }
        protected RedisDb CacheANAME2TNAME { get; set; }
        protected RedisDb CacheID2NAME { get; set; }
        protected RedisDb CacheAddr2ExAddr { get; set; }
#endif

        public IdManager()
        {
            foreach (var cfg in CacheConfig.Instance.CfgDic)
            {
                Global.DbManager.LoadDb(cfg.Value);
            }

#if !CLIENT
            CacheHNAME2ADDR = Global.DbManager.GetDb(CacheConfig.HNAME2ADDR);
            CacheCNAME2ADDR  = Global.DbManager.GetDb(CacheConfig.CNAME2ADDR);
            CacheANAME2HNAME = Global.DbManager.GetDb(CacheConfig.ANAME2HNAME);
            CacheANAME2CNAME = Global.DbManager.GetDb(CacheConfig.ANAME2CNAME);
            CacheANAME2TNAME = Global.DbManager.GetDb(CacheConfig.ANAME2TNAME);
            CacheID2NAME     = Global.DbManager.GetDb(CacheConfig.ID2NAME);
            CacheAddr2ExAddr = Global.DbManager.GetDb(CacheConfig.ADDR2EXTADDR);
#endif

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName.Replace("Server.App", "Fenix.Runtime").Replace("Client.App", "Fenix.Runtime"));
        }

        public bool RegisterHost(Host host, string address, string extAddress, bool isClient)
        {
            if (Global.Host == null)
                Global.Host = host;
            else if (Global.Host.Id != host.Id)
                Debug.Assert(Global.Host.Id == host.Id);

            return AddHost(host.ToRouteData()); 
        }

        public bool AddHost(HostRouteData hostData, bool noReg=false)
        {
            if (!hostData.IsClient)
            {
                string hostName   = hostData.HostName;
                ulong hostId      = hostData.HostId;
                string address    = hostData.HostIntAddr;
                string extAddress = hostData.HostExtAddr;

                mRouteData.mHNAME2ADDR[hostName] = address;
                mRouteData.mADDR2HNAME[address] = hostName;

                //AddAddressID(addrId, hostId);

                AddNameId(hostName, hostId);

#if !CLIENT
                CacheAddr2ExAddr.SetWithoutLock(address, extAddress); 
                CacheHNAME2ADDR.SetWithoutLock(hostName, address); 
#endif

                mRouteData.mIP2EXTIP[Basic.ToIP(address)]    = Basic.ToIP(extAddress);
                mRouteData.mEXTIP2IP[Basic.ToIP(extAddress)] = Basic.ToIP(address);

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
                            mRouteData.UpdateFrom(idAll);
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
                                    mRouteData.UpdateFrom(task.Result.arg1);
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

        void AddNameId(string name, ulong? id=null)
        {
            var newId = Basic.GenID64FromName(name);
            //if(id.HasValue && id != null && newId != id)
            //{
            //    Log.Error("add_name_id_error", name, id, newId);
            //    return;
            //}
            //Log.Error("ADDNAMEID", name, newId, id);
            if (newId != id)
                mRouteData.mID2NAME[newId] = name;
            mRouteData.mNAME2ID[name] = newId;
            if (id != null && id.HasValue)
                mRouteData.mID2NAME[id.Value] = name;

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
    }
}
