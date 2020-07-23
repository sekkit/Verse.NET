using Fenix;
using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

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
