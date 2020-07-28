
using Fenix.Common.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config.Db
{  
    public class DbConfig
    { 
        public static DbEntry account_db = new DbEntry()
        {
            Name = "account",
            Host = "127.0.0.1",
            Port = 7380,
            Key = "ACC",
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };

    }
}
