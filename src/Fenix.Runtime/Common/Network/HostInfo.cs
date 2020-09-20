using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [MessagePackObject]
    public class HostInfo : IMessage
    {
        [Key(0)]
        public ulong HostId { get; set; }

        [Key(1)]
        public string HostName { get; set; }

        [Key(2)]
        public string HostAddr { get; set; }

        [Key(3)]
        public string HostExtAddr { get; set; }

        [Key(4)]
        public bool IsClient { get; set; }
        
        [Key(5)]
        public Dictionary<ulong, string> ServiceId2Name { get; set; }

        [Key(6)]
        public Dictionary<ulong, string> ServiceId2TName { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<HostInfo>(this);
        }

        public new static HostInfo Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<HostInfo>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<HostInfo>.CopyTo(obj, this);
        }
    }
}
