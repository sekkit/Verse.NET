using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DataModel
{
    [MessagePackObject]
    public class MatchData : IMessage
    {
        [Key(0)]
        public Dictionary<int, object> matchData;
    }
}
