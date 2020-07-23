//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using Shared;
using Shared.Protocol;
using System; 
using Shared.DataModel;

namespace Shared.Message
{
    [MessageType(ProtocolCode.LOGIN_REQ)]
    [MessagePackObject]
    public class LoginReq : IMessageWithCallback
    {
        [Key(0)]
        public String username;

        [Key(1)]
        public String password;


        [Key(199)]
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            public ErrCode code;

        }

    }
}

