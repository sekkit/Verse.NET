using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.VOID)]
    public partial class VoidMsg : Msg
    {
        public static VoidMsg Instance { get; set; } = new();

        public override byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}