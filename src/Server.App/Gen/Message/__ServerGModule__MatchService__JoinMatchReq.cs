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
    [MessageType(ProtocolCode.__SERVERGMODULE__MATCHSERVICE__JOIN_MATCH_REQ)]
    [MessagePackObject]
    public class __ServerGModule__MatchService__JoinMatchReq : IMessageWithCallback
    {
        [Key(0)]
        public global::System.String uid { get; set; }

        [Key(1)]
        public global::System.Int32 match_type { get; set; }

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
            return MessagePackSerializer.Serialize<__ServerGModule__MatchService__JoinMatchReq>(this);
        }

        public new static __ServerGModule__MatchService__JoinMatchReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__ServerGModule__MatchService__JoinMatchReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__ServerGModule__MatchService__JoinMatchReq>.CopyTo(obj, this);
        }
    }
}

