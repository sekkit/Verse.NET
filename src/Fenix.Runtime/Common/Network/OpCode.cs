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
        public const uint REMOVE_CLIENT_ACTOR_REQ = 0x16;
        public const uint ON_BEFORE_DISCONNECT_NTF = 0x17;
        public const uint ON_SERVER_ACTOR_ENABLE_NTF = 0x18;
        public const uint SYNC_NTF = 0x20;
        public const uint SYNC_FIELD_NTF = 0x21;
        public const uint SAY_HELLO_REQ = 0x30;
        //public const uint FIND_ACTOR_ID_REQ = 0x40;
        //public const uint FIND_HOST_ID_REQ = 0x41;
        public const uint ADD_HOST_ID_REQ = 0x41;
        public const uint ADD_ACTOR_ID_REQ = 0x42;
        public const uint REMOVE_HOST_ID_REQ = 0x43;
        public const uint REMOVE_ACTOR_ID_REQ = 0x44;
        public const uint ON_ADD_HOST_ID_REQ = 0x46;
        public const uint ON_ADD_ACTOR_ID_REQ = 0x47;
        public const uint ON_REMOVE_HOST_ID_REQ = 0x48;
        public const uint ON_REMOVE_ACTOR_ID_REQ = 0x49;
        public const uint GET_ID_ALL_REQ = 0x50;

        public const uint CALL_ACTOR_METHOD = 0xff;
    }

    public enum DisconnectReason
    {
        DEFAULT = 0x1,
        KICKED = 0x2,
        SERVER_ACTOR_DESTROY = 0x3,
    }
}