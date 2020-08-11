 
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{  
    public class DbConfig : DbConf
    { 
        public const string ACCOUNT = "ACCOUNT";
        public const string USER = "USER";
        public const string RUNTIME = "RUNTIME";
        public const string SEQ     = "SEQ"; 

        public const string key_seq_uid = "key_seq_uid"; 

        public static DbEntry account_db = new DbEntry()
        {
            Name = ACCOUNT,
            Host = "127.0.0.1",
            Port = 7380,
            Key = ACCOUNT,
            Retry = 3,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Kedis"
        };

        public static DbEntry user_db = new DbEntry()
        {
            Name = USER,
            Host = "127.0.0.1",
            Port = 7385,
            Key = USER,
            Retry = 3,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Kedis"
        };

        public static DbEntry runtime_db = new DbEntry()
        {
            Name = RUNTIME,
            Host = "127.0.0.1",
            Port = 7386,
            Key = RUNTIME,
            Retry = 3,
            RetryDelay = 0.1f,
            ValidTime = 60 * 60,
            Type = "Redis"
        };

        public static DbEntry seq_db = new DbEntry()
        {
            Name = SEQ,
            Host = "127.0.0.1",
            Port = 7383,
            Key = SEQ,
            Retry = 3,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public new static void Init()
        {
            AddDbConfig(ACCOUNT, "127.0.0.1", 7380, ACCOUNT, 3, type: "Kedis");
            AddDbConfig(USER, "127.0.0.1", 7380, USER, 3, type: "Kedis");
            AddDbConfig(RUNTIME, "127.0.0.1", 7380, RUNTIME, 3, type: "Redis");
            AddDbConfig(SEQ, "127.0.0.1", 7380, SEQ, 3, type: "Redis"); 
        }
    }
}
