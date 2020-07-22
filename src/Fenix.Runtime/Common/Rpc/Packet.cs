//
using DotNetty.Buffers; 
using Fenix.Common.Utils;
using MessagePack;

namespace Fenix
{  
    [MessagePackObject]
    public class Packet
    {
        [Key(0)]
        public ulong Id { get; set; }

        [Key(1)]
        public uint ProtoCode { get; set; }

        [Key(2)]
        public uint FromHostId { get; set; }
        
        [Key(3)]
        public uint ToHostId { get; set; }

        [Key(2)]
        public uint FromActorId { get; set; }

        [Key(3)]
        public uint ToActorId { get; set; }

        [Key(100)]
        public byte[] Payload { get; set; }

        public static Packet Create(ulong id, uint protoCode, uint fromHostId, uint toHostId, uint fromActorId, uint toActorId, byte[] data)
        {
            var obj = new Packet();
            obj.Id = id;
            obj.ProtoCode = protoCode;
            obj.FromHostId = fromHostId;
            obj.ToHostId = toHostId;
            obj.FromActorId = fromActorId;
            obj.ToActorId = toActorId;
            obj.Payload = data;
            return obj;
        }

        //public static Packet Create(ulong id, uint protoCode, byte[] data)
        //{
        //    var obj = new Packet();
        //    obj.Id = id;
        //    obj.ProtoCode = protoCode;
        //    obj.Payload = data;
        //    return obj;
        //}

        public byte[] Pack()
        {
            var buf = Unpooled.DirectBuffer();
            buf.WriteIntLE((int)this.ProtoCode);
            buf.WriteLongLE((long)this.Id);
            buf.WriteIntLE((int)this.FromActorId);
            buf.WriteIntLE((int)this.ToActorId);
            buf.WriteBytes(this.Payload);
            return buf.ToArray();
        }

        public void Unpack(byte[] bytes)
        {
            //
        }
    } 
}