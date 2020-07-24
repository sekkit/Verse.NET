//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.BIND_CLIENT_ACTOR_REQ)]
    [MessagePackObject]
    public class BindClientActorReq : IMessage
    {
        [Key(0)]
        public String name;

    }
}

