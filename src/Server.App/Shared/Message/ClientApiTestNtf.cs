//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using Shared.Protocol;
using System; 
using UModule;

namespace Shared.Protocol.Message
{
    [MessageType(ProtocolCode.CLIENT_API_TEST_NTF)]
    [MessagePackObject]
    public class ClientApiTestNtf : IMessageWithCallback
    {
        [Key(0)]
        public String uid;

        [Key(1)]
        public Int32 match_type;


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

