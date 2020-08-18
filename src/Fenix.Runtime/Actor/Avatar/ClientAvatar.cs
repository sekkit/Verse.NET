using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [NoCodeGen]
    [ActorType(AType.CLIENT)]
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    public class ClientAvatar : Actor
    {
        public string Uid => this.UniqueName;
    }
}
