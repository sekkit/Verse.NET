//
using Fenix.Common.Rpc;
using MessagePack;
using System;

namespace Fenix.Common
{
    [Flags]
    public enum ProtoCode : uint
    {
        NONE = 0x0,
        PING = 0x1,
        PONG = 0x2,
        GOODBYE = 0x4,

        REGISTER = 0x5,

        CREATE_ACTOR = 0x10,
        MIGRATE_ACTOR = 0x11,

        CALL_ACTOR_METHOD = 0xff,
    }
}