using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Server.UModule
{
    [MessagePackObject]
    public class Account
    {
        [Key(0)]
        public string uid;
    }
}
