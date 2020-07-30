using MessagePack; 
using System.Collections.Generic; 


namespace Fenix.Config
{
    [MessagePackObject]
    public class RuntimeConfig
    {
        [Key(0)]
        public string AppName { get; set; }

        [Key(1)]
        public string ExternalIp { get; set; }

        [Key(2)]
        public string InternalIp { get; set; }

        [Key(3)]
        public int Port { get; set; }

        [Key(4)]
        public List<string> DefaultActorNames { get; set; }

        [Key(5)]
        public long HeartbeatIntervalMS { get; set; }

        [Key(6)]
        public NetworkType ClientNetwork { get; set; }

        public static RuntimeConfig MakeDefaultConfig()
        {
            var obj = new RuntimeConfig();
            obj.ExternalIp = "auto";
            obj.InternalIp = "auto";
            obj.Port = 17777;
            obj.AppName = "Login.App";
            obj.HeartbeatIntervalMS = 5000;
            obj.ClientNetwork = NetworkType.KCP;
            
            obj.DefaultActorNames = new List<string>()
            {
                "LoginService",
                "MatchService",
                "MasterService",
                "ZoneService"
            };
            return obj;
        }
    }
}
