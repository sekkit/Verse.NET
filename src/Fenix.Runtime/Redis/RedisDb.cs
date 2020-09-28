
using Fenix.Common; 
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Newtonsoft.Json;

namespace Fenix.Redis
{
#if USE_REDIS_CORE
    using CSRedis;
    public class RedisDb : IDisposable
    { 
        private CSRedisClient client;

        private RedisHelper helper;

        protected DbEntry conf; 

        public RedisDb(DbEntry db)
        {
            conf = db;
            string connStr = string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}_", conf.Host, conf.Port, conf.Key);
            Log.Info(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}", conf.Host, conf.Port, conf.Key));
            client = new CSRedisClient(connStr);
            helper = new RedisHelper();
            helper.Initialization(client);
        }

        public bool Set(string key, object value, int? expireSec = null)
        { 
            if (value == null)
                return false;
            if(value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
            { 
                if(expireSec == null || !expireSec.HasValue)
                    return this.client.Set(key, ((IMessage)value).PackRaw(), expireSeconds: this.conf.ValidTime);
                return this.client.Set(key, ((IMessage)value).PackRaw(), expireSeconds: expireSec.Value);
            }

            if (expireSec == null || !expireSec.HasValue)
                return this.client.Set(key, value, expireSeconds: this.conf.ValidTime);
            return this.client.Set(key, value, expireSeconds: expireSec.Value);
        }

        public async Task<bool> SetAsync(string key, object value, int? expireSec = null)
        {
            if (value == null)
                return false;
            if (value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
            {
                if (expireSec == null || !expireSec.HasValue)
                    return await this.client.SetAsync(key, ((IMessage)value).PackRaw(), expireSeconds: this.conf.ValidTime);
                return await this.client.SetAsync(key, ((IMessage)value).PackRaw(), expireSeconds: expireSec.Value);
            }

            if (expireSec == null || !expireSec.HasValue)
                return await this.client.SetAsync(key, value, expireSeconds: this.conf.ValidTime);
            return await this.client.SetAsync(key, value, expireSeconds: expireSec.Value);
        }

        public bool HasKey(string key)
        {
            return this.client.Exists(key);
        }

        public T Get<T>(string key)  
        {
            var t = typeof(T);
            if (t == typeof(IMessage) || t.IsSubclassOf(typeof(IMessage)))
            {
                var bytes = this.client.GetBytes(key);
                if (bytes == null)
                    return default(T);
                return (T)(object)RpcUtil.Deserialize(typeof(T), bytes);
            }

            return this.client.Get<T>(key);
        }

        public string Get(string key)
        {
            return this.client.Get(key);
        }

        public async Task<object> GetAsync(Type type, string key)
        { 
            if (type == typeof(IMessage) || type.IsSubclassOf(typeof(IMessage)))
            {
                var bytes = await this.client.GetBytesAsync(key);
                if (bytes == null)
                    return null;
                return RpcUtil.Deserialize(type, bytes);
            }

            return await this.client.GetAsync(key);
        }

        public async Task<T> GetAsync<T>(string key) 
        {
            var t = typeof(T);
            if (t == typeof(IMessage) || t.IsSubclassOf(typeof(IMessage)))
            {
                var bytes = await this.client.GetBytesAsync(key);
                if (bytes == null)
                    return default(T);
                return (T)(object)RpcUtil.Deserialize(typeof(T), bytes);
            }

            return await this.client.GetAsync<T>(key);
        }

        public async Task<string> GetAsync(string key)
        {
            return await this.client.GetAsync(key);
        }

        public long Delete(string key)
        {
            return this.client.Del(key);
        } 

        public async Task<long> DeleteAsync(string key)
        {
            return await this.client.DelAsync(key);
        }

        public long NewSeqId(string key)
        {
            return this.client.IncrBy(key);
        }

        public async Task<string[]> KeysAsync()
        {
            var keys = await this.client.KeysAsync(string.Format("{0}*", conf.Key));
            return keys.Where(m => m != null && !m.Contains("CSRedisClientLock:")).Select(m => m.Substring(conf.Key.Length+1)).ToArray();
        }

        public string[] Keys()
        {
            var keys = this.client.Keys(string.Format("{0}*", conf.Key)); 
            return keys.Where(m=>m!=null && !m.Contains("CSRedisClientLock:")).Select(m => m.Substring(conf.Key.Length+1)).ToArray();
        }
        public void Dispose()
        {
            this.client.Dispose(); 
        }
    }
#else

    using StackExchange.Redis;

    public class RedisDb : IDisposable
    {
        private ConnectionMultiplexer clientMultiplexer;// = ConnectionMultiplexer.Connect("localhost");

        private IDatabase client;

        private RedisHelper helper;

        protected DbEntry conf;

        public string Name => conf.Name;

        public RedisDb(DbEntry db)
        {
            conf = db;
            string connStr = string.Format("{0}:{1},password=,defaultDatabase=0", conf.Host, conf.Port);
            Log.Info(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}", conf.Host, conf.Port, conf.Key));
            clientMultiplexer = ConnectionMultiplexer.Connect(connStr);
            client = clientMultiplexer.GetDatabase();
        }

        protected TimeSpan? DefaultExpiry()
        {
            return this.conf.ValidTime == -1 ? null : new Nullable<TimeSpan>(TimeSpan.FromSeconds(this.conf.ValidTime));
        }

        protected string FormatKey(string key)
        {
            return string.Format("{0}_{1}", this.conf.Key, key);
        }

        public bool SetWithoutLock(string key, object value, int? expireSec = null)
        {
            if (value == null)
                return false;
            bool result = false;
            try
            {
                string redisKey = FormatKey(key); 
                TimeSpan? expireSeconds = (expireSec == null || !expireSec.HasValue) ? DefaultExpiry() : TimeSpan.FromSeconds(expireSec.Value);
                 
                if (value == null)
                    return result;
                if (value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
                    result = this.client.StringSet(redisKey, ((IMessage)value).PackRaw(), expireSeconds);
                else
                    result = this.client.StringSet(redisKey, JsonConvert.SerializeObject(value), expireSeconds); 
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public async Task<bool> SetWithoutLockAsync(string key, object value, int? expireSec = null)
        {
            bool result = false;
            try
            {
                RedisValue token = Guid.NewGuid().ToString();// Environment.MachineName;
                string redisKey = FormatKey(key); 
                TimeSpan? expireSeconds = (expireSec == null || !expireSec.HasValue) ? DefaultExpiry() : TimeSpan.FromSeconds(expireSec.Value);
                
                if (value == null)
                    return result;
                if (value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
                    result = await this.client.StringSetAsync(redisKey, ((IMessage)value).PackRaw(), expireSeconds);
                else
                    result = await this.client.StringSetAsync(redisKey, JsonConvert.SerializeObject(value), expireSeconds);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public Tuple<bool, string> Lock(string key)
        {
            if (key == null)
                return new Tuple<bool, string>(false, null);
            string redisKey = FormatKey(key);
            RedisKey lockKey = redisKey + "_lock";
            RedisValue token = Guid.NewGuid().ToString();// Environment.MachineName;
            try
            {
                Log.Warn("LockKey", lockKey);
                if (client.LockTake(lockKey, token, TimeSpan.FromSeconds(3)))
                    return new Tuple<bool, string>(true, token);
                return new Tuple<bool, string>(false, null);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                client.LockRelease(lockKey, token);
                return new Tuple<bool, string>(false, null);
            }
        }

        public bool Unlock(string key, string token)
        {
            if (key == null)
                return false;
            string redisKey = FormatKey(key);
            RedisKey lockKey = redisKey + "_lock";
            try
            {
                Log.Warn("LockKey", lockKey);
                Log.Info("before_unlock", key, token, client.LockQuery(lockKey));
                var result = client.LockRelease(lockKey, token);
                Log.Info("after_unlock", key, token, client.LockQuery(lockKey));
                return result;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public LockData<T> GetLockData<T>(string key)
        {
            return new LockData<T>(this, key);
        }

        public bool Set(string key, object value, int? expireSec = null)
        {
            if (value == null)
                return false;
            bool result = false;
            string redisKey = FormatKey(key);
            RedisValue token = Guid.NewGuid().ToString();// Environment.MachineName;
            RedisKey lockKey = redisKey + "_lock";
            try
            {
                
                int lockCounter = 0;
                TimeSpan? expireSeconds = (expireSec == null || !expireSec.HasValue) ? DefaultExpiry() : TimeSpan.FromSeconds(expireSec.Value);
                
                while (lockCounter < 3)
                {
                    if (!client.LockTake(lockKey, token, TimeSpan.FromSeconds(3)))
                    {
                        lockCounter++;
                        continue;
                    }

                    try
                    {
                        if (value == null)
                            return result;
                        if (value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
                            result = this.client.StringSet(redisKey, ((IMessage)value).PackRaw(), expireSeconds);
                        else
                            result = this.client.StringSet(redisKey, JsonConvert.SerializeObject(value), expireSeconds);
                    }
                    finally
                    {
                        var lockResult = client.LockRelease(lockKey, token);
                        Log.Info("LockResult", lockResult);
                    }

                    break;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            
            return result;
        }

        public async Task<bool> SetAsync(string key, object value, int? expireSec = null)
        {
            bool result = false;
            try
            {
                RedisValue token = Guid.NewGuid().ToString();// Environment.MachineName;
                string redisKey = FormatKey(key);
                RedisKey lockKey = redisKey + "_lock";
                int lockCounter = 0;
                TimeSpan? expireSeconds = (expireSec == null || !expireSec.HasValue) ? DefaultExpiry() : TimeSpan.FromSeconds(expireSec.Value);
                while (lockCounter < 3)
                {
                    if (!await client.LockTakeAsync(lockKey, token, TimeSpan.FromSeconds(3)))
                    {
                        lockCounter++;
                        continue;
                    }

                    try
                    {
                        if (value == null)
                            return result;
                        if (value is IMessage || value.GetType().IsSubclassOf(typeof(IMessage)))
                            result = await this.client.StringSetAsync(redisKey, ((IMessage)value).PackRaw(), expireSeconds);
                        else
                            result = await this.client.StringSetAsync(redisKey, JsonConvert.SerializeObject(value), expireSeconds);
                    }
                    finally
                    {
                        var lockResult = await client.LockReleaseAsync(lockKey, token);
                        Log.Info("LockResult", lockResult);
                    }

                    break;
                }

                return result;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool HasKey(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                return this.client.KeyExists(redisKey);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> HasKeyAsync(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                return await this.client.KeyExistsAsync(redisKey);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public T Get<T>(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                var t = typeof(T);
                if (t == typeof(IMessage) || t.IsSubclassOf(typeof(IMessage)))
                {
                    var str = this.client.StringGet(redisKey);
                    if (str.IsNull || !str.HasValue)
                        return default(T);
                    return (T)(object)RpcUtil.Deserialize(typeof(T), str);
                }

                string value = this.client.StringGet(redisKey);
                if (value == null)
                    return default(T);
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return default(T);
        }

        public string Get(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                string value = this.client.StringGet(redisKey);
                if (value == null)
                    return null;
                return JsonConvert.DeserializeObject<string>(value);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public object Get(Type type, string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                if (type == typeof(IMessage) || type.IsSubclassOf(typeof(IMessage)))
                {
                    var _value = this.client.StringGet(redisKey);
                    if (_value.IsNull || !_value.HasValue)
                        return null;
                    return RpcUtil.Deserialize(type, _value);
                }

                var value = this.client.StringGet(redisKey);
                if (value.IsNull || !value.HasValue)
                    return null;
                return JsonConvert.DeserializeObject(value, type);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public async Task<object> GetAsync(Type type, string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                if (type == typeof(IMessage) || type.IsSubclassOf(typeof(IMessage)))
                {
                    var _value = await this.client.StringGetAsync(redisKey);
                    if (_value.IsNull || !_value.HasValue)
                        return null;
                    return RpcUtil.Deserialize(type, _value);
                }

                var value = await this.client.StringGetAsync(redisKey);
                if (value.IsNull || !value.HasValue)
                    return null;
                return JsonConvert.DeserializeObject(value, type);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                var t = typeof(T);
                if (t == typeof(IMessage) || t.IsSubclassOf(typeof(IMessage)))
                {
                    var _value = await this.client.StringGetAsync(redisKey);
                    if (_value.IsNull || !_value.HasValue)
                        return default(T);
                    return (T)(object)RpcUtil.Deserialize(typeof(T), _value);
                }

                var value = await this.client.StringGetAsync(redisKey);
                if (value.IsNull || !value.HasValue)
                    return default(T);
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return default(T);
        }

        public async Task<string> GetAsync(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                var value = await this.client.StringGetAsync(redisKey);
                if (value.IsNull || !value.HasValue)
                    return null;
                return JsonConvert.DeserializeObject<string>(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }

        public bool Delete(string key)
        {
            try 
            { 
                string redisKey = FormatKey(key);
                return this.client.KeyDelete(redisKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                return await this.client.KeyDeleteAsync(redisKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        public long NewSeqId(string key)
        {
            try
            {
                string redisKey = FormatKey(key);
                return this.client.StringIncrement(redisKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return 0;
        }

        public async Task<string[]> KeysAsync()
        {
            try
            {
                var s = this.clientMultiplexer.GetServer(this.conf.Host, this.conf.Port);
                List<string> keys = new List<string>();
                await foreach (var r in s.KeysAsync(pattern: string.Format("{0}*", conf.Key)))
                    keys.Add(r);
                //var keys = await this.client..scan.KeysAsync(string.Format("{0}*", conf.Key));
                return keys.Where(m => m != null && !m.EndsWith("_lock")).Select(m => m.Substring(conf.Key.Length + 1)).ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return new string[] { };
        }

        public string[] Keys()
        {
            try {
                var s = this.clientMultiplexer.GetServer(this.conf.Host, this.conf.Port);
                List<string> keys = new List<string>();
                foreach (var r in s.Keys(pattern: string.Format("{0}*", conf.Key)))
                    keys.Add(r);
                //var keys = await this.client..scan.KeysAsync(string.Format("{0}*", conf.Key));
                return keys.Where(m => m != null && !m.EndsWith("_lock")).Select(m => m.Substring(conf.Key.Length + 1)).ToArray();

            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return new string[] { };
        }



        public void Dispose()
        {
            this.clientMultiplexer.Dispose();
        }
    }

#endif
}
