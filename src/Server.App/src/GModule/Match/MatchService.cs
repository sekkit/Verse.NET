using DotNetty.Codecs.Mqtt.Packets;
using Fenix;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

using static Fenix.Common.RpcUtil;

namespace GModule.Match
{
    [RuntimeData(typeof(MatchData))]
    public class MatchService : Actor
    {
        public MatchService(string name): base(name)
        {

        }

        public void onLoad()
        {
            //
        }

        //public new string UniqueName => nameof(MatchService);

        [ServerApi]
        public void add_to_match(string uid, int match_type, Action<MatchCode> callback)
        {   

            callback(MatchCode.OK);
        }
    }
}
