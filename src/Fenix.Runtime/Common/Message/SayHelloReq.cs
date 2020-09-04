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
    [MessageType(OpCode.SAY_HELLO_REQ)]
    [MessagePackObject]
    public class SayHelloReq : IMessageWithCallback
    {

        [Key(0)]

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
            public global::Fenix.HostInfo arg1 { get; set; }

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
            return MessagePackSerializer.Serialize<SayHelloReq>(this);
        }

        public new static SayHelloReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<SayHelloReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<SayHelloReq>.CopyTo(obj, this);
        }
    }
}

