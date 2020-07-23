//
using Fenix.Common.Rpc;
using MessagePack;
using System;

namespace Fenix.Common
{ 
    [Flags]
    public enum ProtoCode : uint
    {
        NONE              = 0x0,
        PING              = 0x1,
        PONG              = 0x2, 
        GOODBYE           = 0x4,

        REGISTER          = 0x5,

        CREATE_ACTOR      = 0x10,
        MIGRATE_ACTOR     = 0x11,

        CALL_ACTOR_METHOD = 0xff,
    }

    //[MessagePackObject]
    //public class CreateActorReq : IMessageWithCallback
    //{
    //    [Key(0)]
    //    public string typeName;

    //    [Key(1)]
    //    public string name;

    //    [MessagePackObject]
    //    public class Callback
    //    {
    //        [Key(0)]
    //        public ErrorCode code;
    //    }

    //    [Key(199)]
    //    public Callback callback
    //    {
    //        get => _callback as Callback;
    //        set => _callback = value;
    //    }
    //}
}