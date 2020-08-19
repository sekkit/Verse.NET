using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{
    public abstract class DbConf
    {
        protected DbConf()
        {
            
        }

        public const string RUNTIME = "RUNTIME";

        protected Dictionary<string, DbEntry> _cfgDic = new Dictionary<string, DbEntry>();

        public Dictionary<string, DbEntry> CfgDic => _cfgDic;

        public DbEntry Get(string dbName)
        {
            if (_cfgDic.TryGetValue(dbName, out var dbEntry))
                return dbEntry;
            return null;
        }

        public DbEntry CreateDbConfig(string dbName, string host, int port, string keyName, int retry = 1, float retryDelay = 0.1f, int validTime = -1, string type = "Redis")
        {
            return new DbEntry()
            {
                Name = dbName,
                Host = host,
                Port = port,
                Key = keyName,
                Retry = retry,
                RetryDelay = retryDelay,
                ValidTime = validTime,
                Type = type
            };
        }

        public void AddDbConfig(string dbName, string host, int port, string keyName, int retry = 1, float retryDelay = 0.1f, int validTime = -1, string type = "Redis")
        {
            _cfgDic[dbName] = CreateDbConfig(dbName, host, port, keyName, retry, retryDelay, validTime, type);
        }

        public static void Init() { }
    }
}
