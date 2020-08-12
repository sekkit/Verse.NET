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
    [MessageType(ProtocolCode.ON_MATCH_OK_REQ)]
    [MessagePackObject]
    public class OnMatchOkReq : IMessage
    {

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<OnMatchOkReq>(this);
        }
        public new static OnMatchOkReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<OnMatchOkReq>(data);
        }
    }
}

