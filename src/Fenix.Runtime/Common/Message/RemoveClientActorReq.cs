//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.REMOVE_CLIENT_ACTOR_REQ)]
    [MessagePackObject]
    public class RemoveClientActorReq : IMessageWithCallback
    {
        [Key(0)]
        public UInt64 actorId { get; set; }

        [Key(1)]
        public DisconnectReason reason { get; set; }

        [Key(2)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {
            [Key(0)]
            [DefaultValue(DefaultErrCode.ERROR)]
            public DefaultErrCode code { get; set; } = DefaultErrCode.ERROR;

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this);
            }
            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<RemoveClientActorReq>(this);
        }
        public new static RemoveClientActorReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<RemoveClientActorReq>(data);
        }
    }
}

