//
using CSRedis;
using System.Collections.Concurrent;
using System.Collections.Generic; 
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
        
        protected ConcurrentDictionary<string, uint> mActor2Id = new ConcurrentDictionary<string, uint>();

        protected ConcurrentDictionary<string, uint> mContainer2Id = new ConcurrentDictionary<string, uint>();

        static readonly string CID2ADDR_KEY = "CID2ADDR";
        static readonly string ADDR2CID_KEY = "ADDR2CID";
        static readonly string AID2CID_KEY = "AID2CID";
        static readonly string CID2AID_KEY = "CID2AID"; 
        static readonly string ANAME2AID_KEY = "ANAME2AID";
        static readonly string CNAME2CID_KEY = "CNAME2CID";

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

        //public void RegisterActorType<T>()
        //{
        //    this.mActor2Type[nameof(T)] = typeof(T);
        //}

        //public void RegisterActorType(Type type)
        //{
        //    this.mActor2Type[type.GetType().Name] = type;
        //}

        public bool RegisterContainer(Container container, string address)
        {
            mContainerId2Addr[container.Id] = address;
            mAddr2ContainerId[address] = container.Id;

            mContainer2Id[container.UniqueName] = container.Id;

            var key = container.Id.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                bool ret = redisClient.Set(address, container.Id);
                ret = redisClient.Set(container.UniqueName, container.Id);
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

        public bool RegisterActor(Actor actor, Container container)
        {
            mActorId2ContainerId[actor.Id] = container.Id;
            if (!mContainerId2ActorId.ContainsKey(container.Id))
                mContainerId2ActorId[container.Id] = new List<uint>();
            mContainerId2ActorId[container.Id].Add(actor.Id);

            mActor2Id[actor.UniqueName] = actor.Id;

            var key = actor.Id.ToString();
            using (var lk = RedisHelper.Lock(key, 3))
            {
                redisClient.Set(actor.UniqueName, actor.Id);
                return redisClient.Set(key, container.Id);
            }
        }

        public uint GetActorId(string name)
        {
            if (mActor2Id.ContainsKey(name))
                return mActor2Id[name];
            var key = name;
            return redisClient.Get<uint>(key);
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
