//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using Shared;
using Shared.Protocol;
using Shared.DataModel;
using System; 

namespace Shared.Message
{
    [MessageType(ProtocolCode.LOGIN_REQ)]
    [MessagePackObject]
    public class LoginReq : IMessageWithCallback
    {
        [Key(0)]
        public String username { get; set; }

        [Key(1)]
        public String password { get; set; }

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
            [DefaultValue(ErrCode.ERROR)]
            public ErrCode code { get; set; } = ErrCode.ERROR;

            [Key(1)]
            public String arg1 { get; set; }

            [Key(2)]
            public UInt64 arg2 { get; set; }

            [Key(3)]
            public String arg3 { get; set; }

            [Key(4)]
            public String arg4 { get; set; }

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
            return MessagePackSerializer.Serialize<LoginReq>(this, RpcUtil.lz4Options);
        }
        public new static LoginReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<LoginReq>(data, RpcUtil.lz4Options);
        }
    }
}

