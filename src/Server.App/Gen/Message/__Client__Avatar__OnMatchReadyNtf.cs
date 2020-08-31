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
    [MessageType(ProtocolCode.__CLIENT__AVATAR__ON_MATCH_READY_NTF)]
    [MessagePackObject]
    public class __Client__Avatar__OnMatchReadyNtf : IMessage
    {
        [Key(0)]
        public global::System.UInt64 room_id { get; set; }

        [Key(1)]
        public global::System.String uid { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__Client__Avatar__OnMatchReadyNtf>(this);
        }

        public new static __Client__Avatar__OnMatchReadyNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__Client__Avatar__OnMatchReadyNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__Client__Avatar__OnMatchReadyNtf>.CopyTo(obj, this);
        }
    }
}

