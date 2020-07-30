using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Shared.DataModel
{
    [MessagePackObject]
    public class Account : IMessage
    {
        [Key(0)]
        public string uid;
    }
}
