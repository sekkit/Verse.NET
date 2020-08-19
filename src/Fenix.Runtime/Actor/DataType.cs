using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public enum DataType : byte
    {
        RUNTIME=0x1,
        PERSIST=0x2,
        VOLATILE=0x3,
    }
}
