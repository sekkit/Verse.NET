
using Fenix.Common.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config.Db
{  
    public class DbConfig
    {
        public static string ACCOUNT = "ACCOUNT";
        public static string SEQ = "SEQ";

        public static string key_seq_uid = "key_seq_uid";
        //public static string key_seq_uid = "key_seq_uid";

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


    }
}
