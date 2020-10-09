using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class HostRouteData : IMessage
    {
        public ulong HostId { get; set; }

        public string HostName { get; set; }

        public string HostIntAddr { get; set; }

        public string HostExtAddr { get; set; }

        public HashSet<ulong> UniqueAddrIds { get; set; } = new HashSet<ulong>();

        public bool IsClient { get; set; }

        public HashSet<ulong> ActorIds { get; set; } = new HashSet<ulong>();

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<HostRouteData>(this);
        }

        public new static HostRouteData Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<HostRouteData>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<HostRouteData>.CopyTo(obj, this);
        }
    }
}
