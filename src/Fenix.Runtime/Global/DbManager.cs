#if !CLIENT

using DotNetty.Common.Utilities; 
using Fenix.Redis;
using Server.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class DbManager
    {
        public DbManager()
        {
        }

        public ConcurrentDictionary<string, RedisDb> DbDic = new ConcurrentDictionary<string, RedisDb>();
        
        public RedisDb LoadDb(DbEntry db)
        {
            DbDic[db.Name] = new RedisDb(db);
            return DbDic[db.Name];
        }

        public RedisDb GetDb(string name)
        {
            DbDic.TryGetValue(name, out var db);
            return db;
        }
    }
}
#endif