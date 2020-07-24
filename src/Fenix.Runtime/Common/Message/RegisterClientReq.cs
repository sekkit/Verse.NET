//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.REGISTER_CLIENT_REQ)]
    [MessagePackObject]
    public class RegisterClientReq : IMessageWithCallback
    {
        [Key(0)]
        public UInt32 hostId;

        [Key(1)]
        public String uniqueName;


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
            public Int32 arg0;

            [Key(1)]
            public HostInfo arg1;

        }

    }
}

