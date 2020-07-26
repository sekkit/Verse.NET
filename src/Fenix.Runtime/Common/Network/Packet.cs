//

using DotNetty.Buffers;
using Fenix.Common;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Net;

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
 
        [IgnoreMember]
        public NetworkType NetType { get; set; }

        [IgnoreMember]
        IMessage _msg;

        [IgnoreMember]
        public IMessage Msg
        {
            get
            {
                if (_msg != null)
                    return _msg;
                try
                {
                    _msg = (IMessage)RpcUtil.Deserialize(MsgType, this.Payload);
                }catch(Exception ex)
                {

                }
                return _msg;
            }
        }

        [IgnoreMember]
        public Type MsgType { get; set; }

        [Key(100)]
        public byte[] Payload { get; set; }

        protected Packet()
        { 
        }

        public static Packet Create(ulong id, uint protoCode, uint fromHostId, uint toHostId, uint fromActorId, uint toActorId, NetworkType netType, Type msgType, byte[] data)
        {
            var obj = new Packet();
            obj.Id = id;
            obj.ProtoCode = protoCode;
            obj.FromHostId = fromHostId;
            obj.ToHostId = toHostId;
            obj.FromActorId = fromActorId;
            obj.ToActorId = toActorId;
            obj.NetType = netType;
            obj.MsgType = msgType;
            obj.Payload = data;
            return obj;
        }

        public byte[] Pack()
        {
            var buf = Unpooled.DirectBuffer();
            buf.WriteIntLE((int)this.ProtoCode);
            buf.WriteLongLE((long)this.Id);
            //buf.WriteIntLE((int)this.FromHostId);
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