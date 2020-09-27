using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Server.DataModel
{
    [MessagePackObject(keyAsPropertyName:true)]
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

        public new static Account Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<Account>(data);
        }

        public override byte[] PackRaw()
        {
            return MessagePackSerializer.Serialize<Account>(this, MessagePackSerializerOptions.Standard);
        }
        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static Account DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<Account>(json);
        }
    }
}
