using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

 
[RpcArg("code")]
public enum ErrCode : Int16
{
    OK = 0,
    ERROR = -1,
} 
