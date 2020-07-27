using Newtonsoft.Json; 
using System.Collections.Generic; 


namespace Fenix.Config
{
    [JsonObject]
    public class RuntimeConfig
    {
        [JsonProperty]
        public string AppName { get; set; } 

        [JsonProperty]
        public string ExternalIp { get; set; }

        [JsonProperty]
        public string InternalIp { get; set; }  

        [JsonProperty]
        public int Port { get; set; } 

        [JsonProperty]
        public List<string> DefaultActorNames { get; set; }

        [JsonProperty]
        public long HeartbeatIntervalMS { get; set; }

        [JsonProperty]
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
