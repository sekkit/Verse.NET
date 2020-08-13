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
    [MessageType(OpCode.ON_BEFORE_DISCONNECT_NTF)]
    [MessagePackObject]
    public class OnBeforeDisconnectNtf : IMessageWithCallback
    {
        [Key(0)]
        public global::Fenix.Common.DisconnectReason reason { get; set; }

        [Key(1)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {

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
            return MessagePackSerializer.Serialize<OnBeforeDisconnectNtf>(this);
        }
        public new static OnBeforeDisconnectNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<OnBeforeDisconnectNtf>(data);
        }
    }
}

