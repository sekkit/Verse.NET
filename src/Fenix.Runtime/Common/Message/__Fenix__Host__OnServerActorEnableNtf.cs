//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.ON_SERVER_ACTOR_ENABLE_NTF)]
    [MessagePackObject]
    public class __Fenix__Host__OnServerActorEnableNtf : IMessage
    {
        [Key(0)]
        public global::System.String actorName { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__Fenix__Host__OnServerActorEnableNtf>(this);
        }

        public new static __Fenix__Host__OnServerActorEnableNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__Fenix__Host__OnServerActorEnableNtf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__Fenix__Host__OnServerActorEnableNtf>.CopyTo(obj, this);
        }
    }
}

