using DotNetty.Codecs.Mqtt.Packets;
using Fenix;
using Fenix.Common;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

using static Fenix.Common.RpcUtil;

namespace GModule.Match
{
    [RuntimeData(typeof(MatchData))]
    public partial class MatchService : Actor
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
        public void JoinMatch(string uid, int match_type, Action<MatchCode> callback)
        {
            Log.Info("Call=>server_api:JoinMatch");
            callback(MatchCode.OK);
        }
    }
}
