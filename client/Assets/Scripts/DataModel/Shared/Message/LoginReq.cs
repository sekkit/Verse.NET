using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.LOGIN)]
    public partial class LoginReq : Msg
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
  
        public override byte[] Pack() => MemoryPackSerializer.Serialize(this); 
    }
}