using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DataModel
{
    [MessagePackObject]
    public class MatchData
    {
        public Dictionary<int, object> matchData;
    }
}
