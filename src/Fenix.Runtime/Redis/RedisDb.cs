using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Redis
{
    public class RedisDb : IDisposable
    {
        public string Prefix;

        private CSRedisClient client;

        private RedisHelper helper;

        public RedisDb(string prefix, string addr, int port)
        {
            this.Prefix = prefix;
            Console.WriteLine(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}_", addr, port, Prefix));
            client = new CSRedisClient(string.Format("{0}:{1},password=,defaultDatabase=0,prefix={2}_", addr, port, Prefix));
            helper = new RedisHelper();
            helper.Initialization(client);
        }

        public bool Set(string key, object value)
        { 
            using (var lk = helper.Lock(key, 3))
            {
                return this.client.Set(key, value);
            }
        }

        public T Get<T>(string key)
        {
            return this.client.Get<T>(key);
        }

        public string Get(string key)
        {
            return this.client.Get(key);
        }

        public long Delete(string key)
        {
            return this.client.Del(key);
        }

        public void Dispose()
        {
            this.client.Dispose(); 
        }
    }
}
