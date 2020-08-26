 
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{  
    public class DbConfig : DbConf
    {
        public static DbConfig Instance = new DbConfig();

        public const string ACCOUNT      = "ACCOUNT";
        public const string USER         = "USER"; 
        public const string SEQ          = "SEQ"; 
        public const string LOGIN        = "LOGIN"; 

        public static string key_seq_uid = "key_seq_uid";

        public new static void Init()
        {
            Instance.AddDbConfig(ACCOUNT, "127.0.0.1", 7380, ACCOUNT, 3, type: "Kedis");
            Instance.AddDbConfig(LOGIN, "127.0.0.1", 7381, LOGIN, 3, validTime: 3600, type: "Redis");
            Instance.AddDbConfig(USER, "127.0.0.1", 7382, USER, 3, type: "Kedis");
            Instance.AddDbConfig(RUNTIME, "127.0.0.1", 7383, RUNTIME, 3, type: "Redis");
            Instance.AddDbConfig(SEQ, "127.0.0.1", 7384, SEQ, 3, type: "Redis");
        }
    }
}
