//

using System;
using System.Dynamic;
using Fenix.Common;
using MessagePack;

namespace Fenix
{ 
    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public ulong Id { get; set; }

        [Key(1)]
        public uint ProtocolId { get; set; }

        [Key(2)]
        public uint fromActorId { get; set; }

        [Key(3)]
        public uint toActorId { get; set; }

        [Key(100)]
        public byte[] Payload { get; set; }

        public static Message Create(ulong id, uint protocolId, uint fromActorId, uint toActorId, byte[] data)
        {
            var obj = new Message();
            obj.Id = id;
            obj.ProtocolId = protocolId;
            obj.fromActorId = fromActorId;
            obj.toActorId = toActorId;
            obj.Payload = data;
            return obj;
        }

        public static Message Create(ulong id, byte protocolId, byte[] data)
        {
            var obj = new Message();
            obj.Id = id;
            obj.ProtocolId = protocolId;
            obj.Payload = data;
            return obj;
        }
    }

    [MessagePackObject]
    public class IMessage
    {
        public byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public virtual bool HasCallback()
        {
            return false;
        }
    }

    [MessagePackObject]
    public class IMessageWithCallback: IMessage
    {
        [IgnoreMember]
        public object _callback;

        public override bool HasCallback()
        {
            return true;
        }
    }
}