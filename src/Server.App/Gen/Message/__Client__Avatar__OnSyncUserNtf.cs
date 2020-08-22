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
    [MessageType(ProtocolCode.__CLIENT__AVATAR__ON_SYNC_USER_NTF)]
    [MessagePackObject]
    public class __Client__Avatar__OnSyncUserNtf : IMessage
    {
        [Key(0)]
        public global::System.Byte[] data { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__Client__Avatar__OnSyncUserNtf>(this);
        }

        public new static __Client__Avatar__OnSyncUserNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__Client__Avatar__OnSyncUserNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__Client__Avatar__OnSyncUserNtf>.CopyTo(obj, this);
        }
    }
}

