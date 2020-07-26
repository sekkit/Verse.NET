using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fenix.Common
{
    [RpcArg("code")]
    [DefaultValue(DefaultErrCode.ERROR)]
    public enum DefaultErrCode: Int16
    {
        OK = 0,
        ERROR = -1
    }
}
