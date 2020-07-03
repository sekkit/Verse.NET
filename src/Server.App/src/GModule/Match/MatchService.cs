using DotNetty.Codecs.Mqtt.Packets;
using Fenix.FACG.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Fenix.Common.RpcUtil;

namespace Fenix.FACG.GModule
{
    [ActorType(typeof(MatchService))]
    public class MatchService: ActorLogic
    { 
        [ServerApi]
        public void add_to_match(string uid, int match_type, Action<MatchCode> callback)
        {

            callback(MatchCode.OK);
        }
    }
}
