using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config.Db
{
    public class CacheConfig
    {
        public const string HID2ADDR = "HID2ADDR";
        public const string ADDR2HID = "ADDR2HID";
        public const string AID2HID = "AID2HID";
        public const string HID2AID = "HID2AID";
        public const string ANAME2AID = "ANAME2AID";
        public const string AID2ANAME = "AID2ANAME";
        public const string AID2TNAME = "AID2TNAME";
        public const string HID2HNAME = "HID2HNAME";
        public const string HNAME2HID = "HNAME2HID"; 

        public static DbEntry HID2ADDR_cache = new DbEntry()
        {
            Name = "HID2ADDR",
            Host = "127.0.0.1",
            Port = 7381,
            Key = HID2ADDR,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry ADDR2HID_cache = new DbEntry()
        {
            Name = "ADDR2HID",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ADDR2HID,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry AID2HID_cache = new DbEntry()
        {
            Name = "AID2HID",
            Host = "127.0.0.1",
            Port = 7381,
            Key = AID2HID,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry HID2AID_cache = new DbEntry()
        {
            Name = "HID2AID",
            Host = "127.0.0.1",
            Port = 7381,
            Key = HID2AID,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry ANAME2AID_cache = new DbEntry()
        {
            Name = "ANAME2AID",
            Host = "127.0.0.1",
            Port = 7381,
            Key = ANAME2AID,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };
         
        public static DbEntry HID2HNAME_cache = new DbEntry()
        {
            Name = "HID2HNAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = HID2HNAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry HNAME2HID_cache = new DbEntry()
        {
            Name = "HNAME2HID",
            Host = "127.0.0.1",
            Port = 7381,
            Key = HNAME2HID,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry AID2ANAME_cache = new DbEntry()
        {
            Name = "AID2ANAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = AID2ANAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        };

        public static DbEntry AID2TNAME_cache = new DbEntry()
        {
            Name = "AID2TNAME",
            Host = "127.0.0.1",
            Port = 7381,
            Key = AID2TNAME,
            Retry = 1,
            RetryDelay = 0.1f,
            ValidTime = -1,
            Type = "Redis"
        }; 
    }
}
