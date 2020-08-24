//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
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
        public global::System.UInt64 actorId { get; set; }

        [Key(1)]
        public global::Fenix.Common.DisconnectReason reason { get; set; }

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
            public global::Fenix.Common.DefaultErrCode code { get; set; } = DefaultErrCode.ERROR;

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this);
            }

            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data);
            }

            public override void UnPack(byte[] data)
            {
                var obj = Deserialize(data);
                Copier<Callback>.CopyTo(obj, this);
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

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<RemoveClientActorReq>.CopyTo(obj, this);
        }
    }
}

