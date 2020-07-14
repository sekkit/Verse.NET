using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GModule.Match
{
    [MessagePackObject]
    public class MatchData
    {
        public Dictionary<int, object> matchData;
    }
}
