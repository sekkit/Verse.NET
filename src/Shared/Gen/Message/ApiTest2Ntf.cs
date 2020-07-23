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
    [MessageType(ProtocolCode.API_TEST2_NTF)]
    [MessagePackObject]
    public class ApiTest2Ntf : IMessage
    {
        [Key(0)]
        public String uid;

        [Key(1)]
        public Int32 match_type;

    }
}

