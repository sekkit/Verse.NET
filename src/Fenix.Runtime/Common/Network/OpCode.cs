//
using Fenix.Common.Rpc;
using MessagePack;
using System;

namespace Fenix.Common
{ 
    public class OpCode
    {
        public const uint NONE = 0x0;
        public const uint PING = 0x1;
        public const uint PONG = 0x2;
        public const uint PARTIAL = 0x3;
        public const uint GOODBYE = 0x4;
        public const uint REGISTER_REQ = 0x5;
        public const uint CREATE_ACTOR_REQ = 0x10;
        public const uint MIGRATE_ACTOR_REQ = 0x11;
        public const uint REMOVE_ACTOR_REQ = 0x12;
        public const uint REGISTER_CLIENT_REQ = 0x13;
        public const uint BIND_CLIENT_ACTOR_REQ = 0x14;
        public const uint RECONNECT_SERVER_ACTOR_NTF = 0x15;
        public const uint CALL_ACTOR_METHOD = 0xff;

        //NONE = 0x0,
        //PING = 0x1,
        //PONG = 0x2,
        //GOODBYE = 0x4,

        //REGISTER = 0x5,

        //CREATE_ACTOR_REQ = 0x10,
        //MIGRATE_ACTOR_REQ = 0x11,

        //CALL_ACTOR_METHOD = 0xff,
    }
}