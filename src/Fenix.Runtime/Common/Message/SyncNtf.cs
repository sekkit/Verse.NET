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
    [MessageType(OpCode.SYNC_NTF)]
    [MessagePackObject]
    public class SyncNtf : IMessage
    {
        [Key(0)]
        public global::System.UInt64 actorId { get; set; }

        [Key(1)]
        public global::System.UInt64 dataKey { get; set; }

        [Key(2)]
        public global::Fenix.DataType dataType { get; set; }

        [Key(3)]
        public global::System.Byte[] data { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<SyncNtf>(this);
        }

        public new static SyncNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<SyncNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<SyncNtf>.CopyTo(obj, this);
        }
    }
}

