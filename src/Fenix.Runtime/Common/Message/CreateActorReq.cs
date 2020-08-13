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
    [MessageType(OpCode.CREATE_ACTOR_REQ)]
    [MessagePackObject]
    public class CreateActorReq : IMessageWithCallback
    {
        [Key(0)]
        public global::System.String typename { get; set; }

        [Key(1)]
        public global::System.String name { get; set; }

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

            [Key(1)]
            public global::System.String arg1 { get; set; }

            [Key(2)]
            public global::System.UInt64 arg2 { get; set; }

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
            return MessagePackSerializer.Serialize<CreateActorReq>(this);
        }
        public new static CreateActorReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<CreateActorReq>(data);
        }
    }
}

