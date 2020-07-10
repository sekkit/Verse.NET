using CSRedis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Fenix.Common;

namespace Fenix
{
    public class IdManager
    {
        public static IdManager Instance = new IdManager();

        protected ConcurrentDictionary<uint, string> mContainerId2Addr = new ConcurrentDictionary<uint, string>();
            
        protected ConcurrentDictionary<string, uint> mAddr2ContainerId = new ConcurrentDictionary<string, uint>();
        
        protected ConcurrentDictionary<uint, uint> mActorId2ContainerId = new ConcurrentDictionary<uint, uint>();
        
        protected ConcurrentDictionary<uint, List<uint>> mContainerId2ActorId = new ConcurrentDictionary<uint, List<uint>>();

        protected ConcurrentDictionary<string, Type> mActor2Type = new ConcurrentDictionary<string, Type>();
        
        private CSRedisClient redisClient { get; set; } 
        
        protected IdManager()
        {
            redisClient = new CSRedisClient(string.Format("{0}:{1},password=,defaultDatabase=0,prefix=ID_", "127.0.0.1", 7382));
            RedisHelper.Initialization(redisClient);

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName);
        }

        ~IdManager()
        {
            redisClient.Dispose();
        }

        //public async Task<bool> RegisterContainerAsync(uint containerId, string address)
        //{
        //    string key = containerId.ToString();
        //    using (var lk = RedisHelper.Lock(key, 3))
        //    {
        //        return await redisClient.SetAsync(key, address);
        //    }
        //}

        public void RegisterActorType<T>()
        {
            this.mActor2Type[nameof(T)] = typeof(T);
        }

        public void RegisterActorType(Type type)
        {
            this.mActor2Type[type.GetType().Name] = type;
        }

        public bool RegisterContainer(uint containerId, string address)
        {
            mContainerId2Addr[containerId] = address;
            mAddr2ContainerId[address] = containerId;
            
            var key = containerId.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                bool ret = redisClient.Set(address, containerId);
                return redisClient.Set(key, address) && ret;
            }
        }

        public string GetContainerAddr(uint containerId)
        {
            if (mContainerId2Addr.ContainsKey(containerId))
                return mContainerId2Addr[containerId];
            var key = containerId.ToString();
            return redisClient.Get(key);
        }

        public uint GetContainerId(string addr)
        {
            if (mAddr2ContainerId.ContainsKey(addr))
                return mAddr2ContainerId[addr];
            return redisClient.Get<uint>(addr);
        }

        public bool RegisterActor(uint actorId, uint containerId)
        {
            mActorId2ContainerId[actorId] = containerId;
            if (!mContainerId2ActorId.ContainsKey(containerId))
                mContainerId2ActorId[containerId] = new List<uint>();
            mContainerId2ActorId[containerId].Add(actorId);
                
            var key = actorId.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                return redisClient.Set(key, containerId);
            }
        }

        public uint GetContainerIdByActorId(uint actorId)
        {
            if (mActorId2ContainerId.ContainsKey(actorId))
                return mActorId2ContainerId[actorId];
            var key = actorId.ToString();
            return redisClient.Get<uint>(key);
        }

        public string GetContainerAddrByActorId(uint actorId)
        {
            uint containerId = 0;
            if (mActorId2ContainerId.ContainsKey(actorId))
            {
                containerId = mActorId2ContainerId[actorId];
                return GetContainerAddr(containerId);
            }
            containerId = GetContainerIdByActorId(actorId);
            return GetContainerAddr(containerId);
        }
    }
}
