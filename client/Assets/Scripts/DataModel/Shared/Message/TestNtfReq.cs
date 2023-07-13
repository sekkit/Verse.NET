using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.TEST_NTF)]
    public partial class TestNtfReq : Msg
    {
        public override byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}