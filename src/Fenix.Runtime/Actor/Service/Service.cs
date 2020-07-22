using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    //Service cannot communicate with client
    //server only
    [ActorType(AType.SERVER)]
    [AccessLevel(ALevel.SERVER)]
    public class Service : Actor
    {
        public Service(string name) : base(name)
        {

        }
    }
}
