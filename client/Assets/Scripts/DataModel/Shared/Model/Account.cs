using MemoryPack;

namespace DataModel.Shared.Model
{
    [MemoryPackable]
    public partial class Account
    {
        public string Uid { get; set; }
        
        public string Password { get; set; }
    }
}