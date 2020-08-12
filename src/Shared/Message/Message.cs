using MessagePack;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Message
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class RegisterDummy
    {
        public ErrCode code;
    }
}
