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
        ERROR = -1, 

        migrate_actor_not_exists = -100,
        create_actor_already_exists = -101,
        create_actor_remote_exists = -102,
        client_host_already_exists = -200
    }
}
