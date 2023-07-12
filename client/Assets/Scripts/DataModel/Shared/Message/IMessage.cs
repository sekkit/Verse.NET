using MemoryPack;

namespace DataModel.Shared.Message
{
    public interface IMessage
    {
        byte[] Pack();// => MemoryPackSerializer.Serialize(this);
    }
}