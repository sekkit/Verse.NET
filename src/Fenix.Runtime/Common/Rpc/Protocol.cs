using DotNetty.Transport.Libuv.Native;
using MessagePack;
using System;
using System.Runtime.Serialization;

namespace Fenix.Common
{ 
    [Flags]
    public enum DefaultProtocol : uint
    {
        PING = 0x1,
        PONG = 0x2,
        GOODBYE = 0x4,
        SPAWN_ACTOR       = 0x5,
        MIGRATE_ACTOR     = 0x6,
        CALL_ACTOR_METHOD = 0x1f,
    }

    [MessagePackObject]
    public class SpawnActorMsg
    {
        [Key(0)]
        public string typeName;

        [Key(1)]
        public string name;

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            public ErrorCode code;
        }

        [Key(199)]
        public Callback callback;
    }
}