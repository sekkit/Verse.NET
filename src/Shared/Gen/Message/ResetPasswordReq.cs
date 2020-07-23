//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using Shared;
using Shared.Protocol;
using Shared.DataModel;
using System; 

namespace Shared.Message
{
    [MessageType(ProtocolCode.RESET_PASSWORD_REQ)]
    [MessagePackObject]
    public class ResetPasswordReq : IMessage
    {
        [Key(0)]
        public String username;

        [Key(1)]
        public String email;

    }
}

