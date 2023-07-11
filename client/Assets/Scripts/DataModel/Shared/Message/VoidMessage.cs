using MemoryPack;

namespace DataModel.Shared.Message
{
    [MemoryPackable]
    public partial class VoidMessage : IMessage
    {
        public static VoidMessage Instance { get; set; } = new();
    }
}