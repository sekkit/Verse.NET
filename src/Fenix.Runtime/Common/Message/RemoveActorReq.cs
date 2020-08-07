//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.REMOVE_ACTOR_REQ)]
    [MessagePackObject]
    public class RemoveActorReq : IMessage
    {
        [Key(0)]
        public UInt64 actorId { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<RemoveActorReq>(this);
        }
        public new static RemoveActorReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<RemoveActorReq>(data);
        }
    }
}

