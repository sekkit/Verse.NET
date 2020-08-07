//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.RECONNECT_SERVER_ACTOR_NTF)]
    [MessagePackObject]
    public class ReconnectServerActorNtf : IMessageWithCallback
    {
        [Key(0)]
        public UInt64 hostId { get; set; }

        [Key(1)]
        public String hostName { get; set; }

        [Key(2)]
        public String hostIP { get; set; }

        [Key(3)]
        public Int32 hostPort { get; set; }

        [Key(4)]
        public UInt64 actorId { get; set; }

        [Key(5)]
        public String actorName { get; set; }

        [Key(6)]
        public String aTypeName { get; set; }

        [Key(7)]

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
            return MessagePackSerializer.Serialize<ReconnectServerActorNtf>(this);
        }
        public new static ReconnectServerActorNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ReconnectServerActorNtf>(data);
        }
    }
}

