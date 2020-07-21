using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public enum MatchCode
    {
        OK = 0,
        MATCH_FAIL = 1
    }

    public class ProtocolCode
    {
        public const uint JOIN_MATCH_REQ = 3350615592;
    }
}
