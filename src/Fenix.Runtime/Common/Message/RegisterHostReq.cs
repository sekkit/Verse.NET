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
    [MessageType(OpCode.REGISTER_HOST_REQ)]
    [MessagePackObject]
    public class RegisterHostReq : IMessageWithCallback
    {
        [Key(0)]
        public global::System.UInt64 hostId { get; set; }

        [Key(1)]
        public global::System.String hostName { get; set; }

        [Key(2)]
        public global::System.String intAddr { get; set; }

        [Key(3)]
        public global::System.String extAddr { get; set; }

        [Key(4)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {
            [Key(0)]
            public global::System.Boolean arg0 { get; set; }

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
            return MessagePackSerializer.Serialize<RegisterHostReq>(this);
        }

        public new static RegisterHostReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<RegisterHostReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<RegisterHostReq>.CopyTo(obj, this);
        }
    }
}

