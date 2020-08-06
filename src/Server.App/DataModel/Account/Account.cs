using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Server.DataModel
{
    [MessagePackObject]
    public class Account : IMessage
    {
        [Key(0)]
        public string uid;

        [Key(1)]
        public string username;

        [Key(2)]
        public string password;

        [Key(3)]
        public string email;

        [Key(4)]
        public string phone;

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<Account>(this);
        }
    }
}
