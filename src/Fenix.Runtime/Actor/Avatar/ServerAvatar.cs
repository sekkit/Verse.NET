using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix 
{
    [NoCodeGen]
    [ActorType(AType.SERVER)]
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    public class ServerAvatar: Actor
    {
        public string Uid => this.UniqueName; 
    }
}