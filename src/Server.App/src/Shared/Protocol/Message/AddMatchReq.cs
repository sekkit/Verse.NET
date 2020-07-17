using Fenix;
using Fenix.Common.Rpc;
using MessagePack;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Protocol.Message
{
    [MessagePackObject]
    public class JoinMatchReq : IMessageWithCallback
    {
        [Key(0)]
        public string uid;

        [Key(1)]
        public int? match_type;

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            public MatchCode code;
        }

        [Key(199)]
        public Callback callback => _callback as Callback;
    }
}
