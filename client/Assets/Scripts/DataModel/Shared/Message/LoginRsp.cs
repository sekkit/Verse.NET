using DataModel.Shared.Model;
using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.LOGIN)]
    public partial class LoginRsp : Msg
    { 
        public int RetCode { get; set; }
        
        public string Uid { get; set; }
        
        public byte[] UserBytes { get; set; }
        
        public override byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}