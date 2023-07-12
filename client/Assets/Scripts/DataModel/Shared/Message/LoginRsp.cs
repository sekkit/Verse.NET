using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.LOGIN)]
    public partial class LoginRsp : IMessage
    { 
        public int RetCode { get; set; }
        
        public string Uid { get; set; }
        
        public byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}