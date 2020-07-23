using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Message
{
    [MessagePackObject]
    public class CreateActorReq : IMessageWithCallback
    {
        [Key(0)]
        public string typeName;

        [Key(1)]
        public string name;

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            public DefaultErrCode code;
        }

        [Key(199)]
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        }
    }
}
