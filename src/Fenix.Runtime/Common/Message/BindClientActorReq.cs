//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.BIND_CLIENT_ACTOR_REQ)]
    [MessagePackObject]
    public class BindClientActorReq : IMessageWithCallback
    {
        [Key(0)]
        public String actorName { get; set; }


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
            [DefaultValue(DefaultErrCode.ERROR)]
            public DefaultErrCode code { get; set; } = DefaultErrCode.ERROR;

        }

    }
}

