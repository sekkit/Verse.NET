using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GModule
{
    [MessagePackObject]
    public class MatchData
    {
        public Dictionary<int, object> matchData;
    }
}
