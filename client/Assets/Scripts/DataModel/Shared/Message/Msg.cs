using MemoryPack;

namespace DataModel.Shared.Message
{ 
    public abstract class Msg
    {
        public ulong RpcId { get; set; } = 0;
        
        public abstract byte[] Pack();
    }
}