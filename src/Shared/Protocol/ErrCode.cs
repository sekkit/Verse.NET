using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Protocol
{
    [RpcArg("code")]
    [DefaultValue(ErrCode.ERROR)]
    public enum ErrCode : Int16
    {
        OK = 0,
        ERROR = -1,
    }
}
