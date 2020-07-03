using CSRedis;
using Fenix.Common;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Fenix.Global
{
    public class Global
    {
        public static Global Instance = new Global();

        protected ConcurrentDictionary<string, Type> ActorTypeMap = new ConcurrentDictionary<string, Type>();
        
        CSRedisClient redisClient { get; set; }

        protected Global()
        {
            redisClient = new CSRedisClient(string.Format("{0}:{1},password=,defaultDatabase=0,prefix=ID_", "127.0.0.1", 7382));
            RedisHelper.Initialization(redisClient);

            var assembly = typeof(Global).Assembly;
            Log.Info(assembly.FullName);
        }

        ~Global()
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
            this.ActorTypeMap[nameof(T)] = typeof(T);
        }

        public void RegisterActorType(Type type)
        {
            this.ActorTypeMap[type.GetType().Name] = type;
        }

        public bool RegisterContainer(uint containerId, string address)
        {
            string key = containerId.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                return redisClient.Set(key, address);
            }
        }

        public string GetContainerAddr(uint containerId)
        {
            string key = containerId.ToString();
            return redisClient.Get(key);
        }

        public bool RegisterActor(uint actorId, uint containerId)
        {
            string key = actorId.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                return redisClient.Set(key, containerId);
            }
        }

        public uint GetContainerIdByActorId(uint actorId)
        {
            string key = actorId.ToString();
            return redisClient.Get<uint>(key);
        }

        public string GetContainerAddrByActorId(uint actorId)
        {
            uint containerId = GetContainerIdByActorId(actorId);
            return GetContainerAddr(containerId);
        }
    }
}
