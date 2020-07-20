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
        public const uint ADD_TO_MATCH_REQ = 247041346;
    }
}
