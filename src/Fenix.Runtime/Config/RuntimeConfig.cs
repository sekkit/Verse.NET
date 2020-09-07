using MessagePack;
using System;
using System.Collections.Generic; 


namespace Fenix.Config
{
    [MessagePackObject]
    [Serializable]
    public class RuntimeConfig
    {
        [Key(0)]
        public string AppName { get; set; }
#if !CLIENT
        [Key(1)]
        public string ExternalIP { get; set; }

        [Key(2)]
        public string InternalIP { get; set; }
#else
        [Key(3)]
        public string HostIP { get; set; }
#endif

        [Key(4)]
        public int Port { get; set; }

        [Key(5)]
        public List<string> DefaultActorNames { get; set; }

        [Key(6)]
        public long HeartbeatIntervalMS { get; set; }
#if !CLIENT
        [Key(7)]
        public bool DuplexMode { get; set; } = false;
#endif
        [Key(8)]
        public NetworkType ClientNetwork { get; set; } = NetworkType.TCP;

        [Key(9)]
        public NetworkType ServerNetwork { get; set; } = NetworkType.TCP;

        [IgnoreMember]
        public readonly int MAX_PACKET_SIZE = 64*1024;

#if !CLIENT
        public static RuntimeConfig MakeDefaultServerConfig()
        {
            var obj = new RuntimeConfig();
            obj.ExternalIP = "auto";
            obj.InternalIP = "auto";
            obj.Port = 17777;
            obj.AppName = "Login.App";
            obj.HeartbeatIntervalMS = 5000;
            obj.ClientNetwork = NetworkType.TCP;
            obj.DuplexMode = false;

            obj.DefaultActorNames = new List<string>()
            {
                "LoginService",
                "MatchService",
                "MasterService",
                "ZoneService"
            };
            return obj;
        }
#else
        public static RuntimeConfig MakeDefaultClientConfig()
        {
            var obj = new RuntimeConfig();
            obj.HostIP = "127.0.0.1";
            obj.Port = 17777;
            obj.AppName = "Client.App";
            obj.HeartbeatIntervalMS = 5000;
            obj.ClientNetwork = NetworkType.TCP;

            obj.DefaultActorNames = new List<string>()
            {
            };
            return obj;
        }
#endif
    }
}
