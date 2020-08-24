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
    [MessageType(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__LOGIN_REQ)]
    [MessagePackObject]
    public class __ServerGModule__LoginService__LoginReq : IMessageWithCallback
    {
        [Key(0)]
        public global::System.String username { get; set; }

        [Key(1)]
        public global::System.String password { get; set; }

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
            public global::Shared.Protocol.ErrCode code { get; set; } = ErrCode.ERROR;

            [Key(1)]
            public global::System.String arg1 { get; set; }

            [Key(2)]
            public global::System.UInt64 arg2 { get; set; }

            [Key(3)]
            public global::System.String arg3 { get; set; }

            [Key(4)]
            public global::System.String arg4 { get; set; }

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this);
            }

            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data);
            }

            public override void UnPack(byte[] data)
            {
                var obj = Deserialize(data);
                Copier<Callback>.CopyTo(obj, this);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__ServerGModule__LoginService__LoginReq>(this);
        }

        public new static __ServerGModule__LoginService__LoginReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__ServerGModule__LoginService__LoginReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__ServerGModule__LoginService__LoginReq>.CopyTo(obj, this);
        }
    }
}

