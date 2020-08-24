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
    [MessageType(ProtocolCode.__SERVERUMODULE__AVATAR__ON_MATCH_OK_REQ)]
    [MessagePackObject]
    public class __ServerUModule__Avatar__OnMatchOkReq : IMessage
    {

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__ServerUModule__Avatar__OnMatchOkReq>(this);
        }

        public new static __ServerUModule__Avatar__OnMatchOkReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__ServerUModule__Avatar__OnMatchOkReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__ServerUModule__Avatar__OnMatchOkReq>.CopyTo(obj, this);
        }
    }
}

