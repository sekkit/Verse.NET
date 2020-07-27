#if !CLIENT

using DotNetty.Common.Utilities; 
using Fenix.Redis; 
using Server.Config.Db; 
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

        public void LoadDb(DbEntry db)
        {
            DbDic[db.Name] = new RedisDb(db);
        }

        public RedisDb GetDb(string name)
        {
            DbDic.TryGetValue(name, out var db);
            return db;
        }
    }
}
#endif