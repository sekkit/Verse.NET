using Fenix;
using Fenix.Common.Attributes;

namespace Server.GModule.Master
{
    [AccessLevel(ALevel.SERVER)]
    public partial class MasterService : Service
    {
        public MasterService(string name) : base(name)
        { 
            
        } 
    }
}
