//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using Shared.Protocol;
using System; 
using UModule;

namespace Shared.Protocol.Message
{
    [MessageType(ProtocolCode.FIND_MATCH_REQ)]
    [MessagePackObject]
    public class FindMatchReq : IMessageWithCallback
    {
        [Key(0)]
        public String uid;


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
            public ErrCode code;

            [Key(1)]
            public Account user;

        }

    }
}

