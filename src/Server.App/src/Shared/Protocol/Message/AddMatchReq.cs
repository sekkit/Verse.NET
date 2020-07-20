//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 

namespace Shared.Protocol.Message
{
    [MessageType(ProtocolCode.ADD_TO_MATCH_REQ)]
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
        public Callback callback
        {
	        get => _callback as Callback;
	        set => _callback = value;
        } 
    }
}
