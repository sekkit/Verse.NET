using Fenix.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config.Db
{ 
    public class DbEntry
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Key { get; set; }

        public int Retry { get; set; }

        public float RetryDelay { get; set; }

        public int ValidTime { get; set; }

        public string Type { get; set; }
    }
     

    public class DbConfig
    { 
        public static DbEntry account_db = new DbEntry()
        {
            Name = "account",
            Host = "127.0.0.1",
            Port = 7381,
            Key = "ACC",
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

    }
}
