using Fenix;
using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GModule
{
    [AccessLevel(ALevel.SERVER)]
    public partial class ZoneService : Service
    {
        public ZoneService(string name): base(name)
        { }


    }
}
