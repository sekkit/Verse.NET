using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Rpc
{
    [MessagePackObject]
    public class IMessage
    {
        public byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public virtual bool HasCallback()
        {
            return false;
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
    }
}
