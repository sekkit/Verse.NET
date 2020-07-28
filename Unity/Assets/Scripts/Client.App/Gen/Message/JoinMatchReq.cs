//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using Shared;
using Shared.Protocol;
using Shared.DataModel;
using System; 

namespace Shared.Message
{
    [MessageType(ProtocolCode.JOIN_MATCH_REQ)]
    [MessagePackObject]
    public class JoinMatchReq : IMessageWithCallback
    {
        [Key(0)]
        public String uid { get; set; }

        [Key(1)]
        public Int32 match_type { get; set; }


        [Key(199)]
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            [DefaultValue(ErrCode.ERROR)]
            public ErrCode code { get; set; } = ErrCode.ERROR;

        }

    }
}

