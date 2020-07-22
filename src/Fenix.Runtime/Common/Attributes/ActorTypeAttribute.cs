using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    [Flags]
    public enum AType : int
    {
        CLIENT = 0x1,
        SERVER = 0x1<<1,
        //CLIENT_AND_SERVER = CLIENT | SERVER
    }

    public class ActorTypeAttribute : Attribute
    {
        public AType AType;

        public ActorTypeAttribute(AType aType)
        {
            this.AType = aType;
        }
    }
}
