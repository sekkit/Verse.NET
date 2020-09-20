using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    [MessagePackObject]
    public class ActorInfo : IMessage
    {
        [Key(1)]
        public ulong ActorId { get; set; }

        [Key(2)]
        public string ActorName { get; set; }

        [Key(3)]
        public HostInfo HostInfo { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<ActorInfo>(this);
        }

        public new static ActorInfo Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ActorInfo>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<ActorInfo>.CopyTo(obj, this);
        }
    }
}
