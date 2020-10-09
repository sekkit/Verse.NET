using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ActorRouteData : IMessage
    {
        public ulong ActorId { get; set; }

        public string ActorName { get; set; }

        public string ActorTypeName { get; set; }

        public ulong HostId { get; set; }

        public string HostName { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<ActorRouteData>(this);
        }

        public new static ActorRouteData Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ActorRouteData>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<ActorRouteData>.CopyTo(obj, this);
        }
    }
}
