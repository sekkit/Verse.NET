using CSRedis;
using Fenix.Common; 
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fenix.Redis
{
    public class RedisDb : IDisposable
    { 
        private CSRedisClient client;

        private RedisHelper helper;

        protected DbEntry conf; 

        //public RedisDb(string prefix, string addr, int port)
        //{
        //    //this.Prefix = prefix;
        //    Console.WriteLine(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}_", addr, port, Prefix));
        //    client = new CSRedisClient(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}_", addr, port, Prefix));
        //    helper = new RedisHelper();
        //    helper.Initialization(client);
        //}

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
                    return this.client.Set(key, ((IMessage)value).Pack(), expireSeconds: this.conf.ValidTime);
                return this.client.Set(key, ((IMessage)value).Pack(), expireSeconds: expireSec.Value);
            }

            if (expireSec == null || !expireSec.HasValue)
                return this.client.Set(key, value, expireSeconds: this.conf.ValidTime);
            return this.client.Set(key, value, expireSeconds: expireSec.Value);
        }

        public bool HasKey(string key)
        {
            return this.client.Exists(key);
        }

        //public T Get<T>(string key) where T: IMessage
        //{
        //    var t = typeof(T);
        //    if(t == typeof(IMessage) || t.IsSubclassOf(typeof(IMessage)))
        //    {
        //        var bytes = this.client.GetBytes(key);
        //        if (bytes == null)
        //            return null;
        //        return RpcUtil.Deserialize<T>(bytes);
        //    }

        //    return this.client.Get<T>(key);
        //}

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
}
