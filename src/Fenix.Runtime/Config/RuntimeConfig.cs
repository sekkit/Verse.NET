using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
