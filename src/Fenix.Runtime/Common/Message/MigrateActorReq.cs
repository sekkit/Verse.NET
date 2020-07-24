//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.MIGRATE_ACTOR_REQ)]
    [MessagePackObject]
    public class MigrateActorReq : IMessage
    {
        [Key(0)]
        public UInt32 actorId;

    }
}

