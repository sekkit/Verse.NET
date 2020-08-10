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
    [MessageType(OpCode.REGISTER_CLIENT_REQ)]
    [MessagePackObject]
    public class RegisterClientReq : IMessageWithCallback
    {
        [Key(0)]
        public UInt64 hostId { get; set; }

        [Key(1)]
        public String hostName { get; set; }

        [Key(2)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {
            [Key(0)]
            [DefaultValue(DefaultErrCode.ERROR)]
            public DefaultErrCode code { get; set; } = DefaultErrCode.ERROR;

            [Key(1)]
            public HostInfo arg1 { get; set; }

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this, RpcUtil.lz4Options);
            }
            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data, RpcUtil.lz4Options);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<RegisterClientReq>(this, RpcUtil.lz4Options);
        }
        public new static RegisterClientReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<RegisterClientReq>(data, RpcUtil.lz4Options);
        }
    }
}

