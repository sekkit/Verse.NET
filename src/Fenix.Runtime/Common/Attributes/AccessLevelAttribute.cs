using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    [Flags]
    public enum ALevel
    {
        SERVER=0x1,
        CLIENT=0x1<<1,
        CLIENT_AND_SERVER = SERVER|CLIENT,
    }

    public class AccessLevelAttribute : Attribute
    {
        public ALevel AccessLevel;

        public AccessLevelAttribute(ALevel lvl)
        {
            AccessLevel = lvl;
        }
    }
}
