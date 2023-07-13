using System;
using System.Collections.Generic;
using System.Text;
using MemoryPack;
using Newtonsoft.Json;

namespace DataModel.Shared.Model
{  
    [Serializable]
    public partial class User: Model
    { 
        public string Uid
        {
            get => Get<string>("uid");
            set => this["uid"] = value;
        }

        public string Nickname
        {
            get => Get<string>("nickname");
            set => this["nickname"] = value;
        }

        public byte[] Pack() => MemoryPackSerializer.Serialize(this);

        public void MergeFrom(string fieldName, byte[] value)
        {
            var obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), this[fieldName].GetType());
            this[fieldName] = obj;
        }
    }
}