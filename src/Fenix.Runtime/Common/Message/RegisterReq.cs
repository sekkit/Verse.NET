//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.REGISTER_REQ)]
    [MessagePackObject]
    public class RegisterReq : IMessage
    {
        [Key(0)]
        public UInt32 hostId { get; set; }

        [Key(1)]
        public String hostName { get; set; }

    }
}

