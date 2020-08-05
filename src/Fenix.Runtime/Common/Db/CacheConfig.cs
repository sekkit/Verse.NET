using Fenix.Common.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config.Db
{
    public class CacheConfig
    {
        public const string HNAME2ADDR = "HNAME2ADDR";
        public const string ANAME2HNAME = "ANAME2HNAME";
        public const string ANAME2TNAME = "ANAME2TNAME";
        public const string ANAME2CNAME = "ANAME2CNAME";
        public const string ID2NAME     = "ID2NAME";
        public const string ADDR2EXTADDR = "ADDR2EXTADDR";
 
        public static DbEntry HNAME2ADDR_cache = new DbEntry()
        {
            Name = "HNAME2ADDR",
            Host = "127.0.0.1",
            Port = 7381,
            Key = HNAME2ADDR,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
        public static DbEntry ANAME2HNAME_cache = new DbEntry()
        {
            Name = "ANAME2HNAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ANAME2HNAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
        public static DbEntry ANAME2TNAME_cache = new DbEntry()
        {
            Name = "ANAME2TNAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ANAME2TNAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
        public static DbEntry ID2NAME_cache = new DbEntry()
        {
            Name = "ID2NAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ID2NAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
        public static DbEntry ANAME2CNAME_cache = new DbEntry()
        {
            Name = "ANAME2CNAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ANAME2CNAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
        public static DbEntry ADDR2EXTADDR_cache = new DbEntry()
        {
            Name = "ADDR2EXTADDR",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ADDR2EXTADDR,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = 15,
            Type = "Redis"
        };
    }
}
