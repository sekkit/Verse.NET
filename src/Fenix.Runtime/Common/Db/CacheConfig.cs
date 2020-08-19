
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{ 
    public class CacheConfig : DbConf
    {
        public static CacheConfig Instance = new CacheConfig();

        public const string HNAME2ADDR = "HNAME2ADDR";
        public const string ANAME2HNAME = "ANAME2HNAME";
        public const string ANAME2TNAME = "ANAME2TNAME";
        public const string ANAME2CNAME = "ANAME2CNAME";
        public const string ID2NAME     = "ID2NAME";
        public const string ADDR2EXTADDR = "ADDR2EXTADDR"; 

        public new static void Init()
        {
            Instance.AddDbConfig(HNAME2ADDR, "127.0.0.1", 7381, HNAME2ADDR, validTime: 15);
            Instance.AddDbConfig(ANAME2HNAME, "127.0.0.1", 7381, ANAME2HNAME, validTime: 15);
            Instance.AddDbConfig(ANAME2TNAME, "127.0.0.1", 7381, ANAME2TNAME, validTime: 15);
            Instance.AddDbConfig(ID2NAME, "127.0.0.1", 7381, ID2NAME, validTime: 15);
            Instance.AddDbConfig(ANAME2CNAME, "127.0.0.1", 7381, ANAME2CNAME, validTime: 15);
            Instance.AddDbConfig(ADDR2EXTADDR, "127.0.0.1", 7381, ADDR2EXTADDR, validTime: 15);
        }
    }
}
