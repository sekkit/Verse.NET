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
    [MessageType(OpCode.RECONNECT_SERVER_ACTOR_NTF)]
    [MessagePackObject]
    public class ReconnectServerActorNtf : IMessageWithCallback
    {
        [Key(0)]
        public global::System.UInt64 hostId { get; set; }

        [Key(1)]
        public global::System.String hostName { get; set; }

        [Key(2)]
        public global::System.String hostIP { get; set; }

        [Key(3)]
        public global::System.Int32 hostPort { get; set; }

        [Key(4)]
        public global::System.UInt64 actorId { get; set; }

        [Key(5)]
        public global::System.String actorName { get; set; }

        [Key(6)]
        public global::System.String aTypeName { get; set; }

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
            public global::Fenix.Common.DefaultErrCode code { get; set; } = DefaultErrCode.ERROR;

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this, RpcUtil.lz4Options);
            }
            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data, RpcUtil.lz4Options);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<ReconnectServerActorNtf>(this, RpcUtil.lz4Options);
        }
        public new static ReconnectServerActorNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ReconnectServerActorNtf>(data, RpcUtil.lz4Options);
        }
    }
}

