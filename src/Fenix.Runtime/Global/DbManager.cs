#if !CLIENT

using DotNetty.Common.Utilities;
using Fenix.Common.Db;
using Fenix.Redis;  
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class DbManager
    {
        static UInt32 fakeId = 1;
        public DbManager()
        {
        }

        //public static DbManager Instance = new DbManager();

        public ConcurrentDictionary<string, RedisDb> DbDic = new ConcurrentDictionary<string, RedisDb>();

        public string CreateUid()
        {
            string uid = fakeId.ToString();
            fakeId++;
            return uid;
        }

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