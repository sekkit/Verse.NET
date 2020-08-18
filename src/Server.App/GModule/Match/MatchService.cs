using DotNetty.Codecs.Mqtt.Packets;
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Server.UModule;
using Shared.DataModel;
using Server.DataModel;

namespace Server.GModule
{
    [AccessLevel(ALevel.SERVER)]
    [RuntimeData(typeof(MatchData))]
    public partial class MatchService : Service
    { 
        protected override void onLoad()
        {
        }

        //public new string UniqueName => nameof(MatchService);

        [ServerApi]
        public void JoinMatch(string uid, int match_type, Action<ErrCode> callback)
        {
            Log.Info("Call=>server_api:JoinMatch");
            callback(ErrCode.OK);
        } 

        [ServerOnly]
        [CallbackArgs("code", "user")]
        public void FindMatch(string uid, Action<ErrCode, Account> callback)
        {
            callback(ErrCode.OK, null);
        }
    }
}
