

using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.ON_TEST)]
    public partial class TestNtf : Msg
    {
        public string TestMsg { get; set; }
 
        public override byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}