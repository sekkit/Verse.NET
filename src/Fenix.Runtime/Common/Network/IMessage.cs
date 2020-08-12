using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Rpc
{
    [MessagePackObject]
    public class IMessage
    {
        public IMessage()
        { }

        //public abstract byte[] Pack();

        public virtual byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);//, Utils.RpcUtil.lz4Options);
        }

        //public virtual void UnPack(byte[] data)
        //{
            
        //}

        public virtual string ToJson()
        {
           return MessagePackSerializer.SerializeToJson(this);
        }

        //public virtual void FromJson(string json)
        //{
        //    MessagePackSerializer.ConvertFromJson(json, ref this);
        //}

        public static IMessage Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<IMessage>(data);
        }

        public virtual bool HasCallback()
        {
            return false;
        }

        public virtual object GetCallbackMsg()
        {
            return null;
        }
    }

    [MessagePackObject]
    public class IMessageWithCallback : IMessage
    {
        [IgnoreMember]
        public object _callback;

        public override bool HasCallback()
        {
            return true;
        }

        public override object GetCallbackMsg()
        {
            return _callback;
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}
