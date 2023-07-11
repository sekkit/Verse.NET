using MemoryPack;

namespace DataModel.Shared.Model
{
    [MemoryPackable]
    public partial class User
    {
        public string Uid { get; set; }
        
        public string Nickname { get; set; }
        
        
    }
}