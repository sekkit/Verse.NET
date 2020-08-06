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
        public ulong FromHostId { get; set; }
        
        [Key(3)]
        public ulong ToHostId { get; set; }

        [Key(4)]
        public ulong FromActorId { get; set; }

        [Key(5)]
        public ulong ToActorId { get; set; }
 
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
                    _msg = RpcUtil.Deserialize(MsgType, this.Payload);
                }catch(Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                return _msg;
            }
        }

        [IgnoreMember]
        public Type MsgType { get; set; }

        [Key(6)]
        public byte[] Payload { get; set; }

        public Packet()
        { 
        }

        public static Packet Create(ulong id, uint protoCode, ulong fromHostId, ulong toHostId, ulong fromActorId, ulong toActorId, NetworkType netType, Type msgType, byte[] data)
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
            buf.WriteLongLE((long)this.FromActorId);
            buf.WriteLongLE((long)this.ToActorId); 
            buf.WriteBytes(this.Payload);
            var bytes = buf.ToArray();
            //buf.Release();
            return bytes;
        }

        public void Unpack(byte[] bytes)
        {
            //
        }
    } 
}