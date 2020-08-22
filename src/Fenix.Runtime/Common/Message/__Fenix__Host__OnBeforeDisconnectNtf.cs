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
    public class __Fenix__Host__OnBeforeDisconnectNtf : IMessageWithCallback
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

            public override void UnPack(byte[] data)
            {
                var obj = Deserialize(data);
                Copier<Callback>.CopyTo(obj, this);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__Fenix__Host__OnBeforeDisconnectNtf>(this);
        }

        public new static __Fenix__Host__OnBeforeDisconnectNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__Fenix__Host__OnBeforeDisconnectNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__Fenix__Host__OnBeforeDisconnectNtf>.CopyTo(obj, this);
        }
    }
}

