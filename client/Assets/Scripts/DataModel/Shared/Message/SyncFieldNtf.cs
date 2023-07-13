using MemoryPack;
using Module.Shared;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    [Protocol(ProtoCode.ON_SYNC_FIELD)]
    public partial class SyncFieldNtf : Msg
    {
        public string Key { get; set; }
        
        public byte[] Value { get; set; }
 
        public override byte[] Pack() => MemoryPackSerializer.Serialize(this);
    }
}