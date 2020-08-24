//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using Shared;
using Shared.Protocol;
using Shared.DataModel;
using System; 

namespace Shared.Message
{
    [MessageType(ProtocolCode.__CLIENT__AVATAR__API_TEST_NTF)]
    [MessagePackObject]
    public class __Client__Avatar__ApiTestNtf : IMessageWithCallback
    {
        [Key(0)]
        public global::System.String uid { get; set; }

        [Key(1)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {
            [Key(0)]
            public global::Shared.Protocol.ErrCode code { get; set; } = Shared.Protocol.ErrCode.ERROR;

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
            return MessagePackSerializer.Serialize<__Client__Avatar__ApiTestNtf>(this);
        }

        public new static __Client__Avatar__ApiTestNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__Client__Avatar__ApiTestNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__Client__Avatar__ApiTestNtf>.CopyTo(obj, this);
        }
    }
}

