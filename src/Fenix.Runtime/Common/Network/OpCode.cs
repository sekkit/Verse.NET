//
using Fenix.Common.Rpc;
using MessagePack;
using System;

namespace Fenix.Common
{ 
    public class OpCode
    {
        public const int NONE = 0x0;
        public const int PING = 0x1;
        public const int PONG = 0x2;
        public const int PARTIAL = 0x3;
        public const int GOODBYE = 0x4;
        public const int REGISTER_REQ = 0x5;
        public const int CREATE_ACTOR_REQ = 0x10;
        public const int MIGRATE_ACTOR_REQ = 0x11;
        public const int REMOVE_ACTOR_REQ = 0x12;
        public const int REGISTER_CLIENT_REQ = 0x13;
        public const int BIND_CLIENT_ACTOR_REQ = 0x14;
        public const int RECONNECT_SERVER_ACTOR_NTF = 0x15;
        public const int REMOVE_CLIENT_ACTOR_REQ = 0x16;
        public const int ON_BEFORE_DISCONNECT_NTF = 0x17;
        public const int ON_SERVER_ACTOR_ENABLE_NTF = 0x18;
        public const int SYNC_NTF = 0x20;
        public const int SYNC_FIELD_NTF = 0x21;
        public const int SAY_HELLO_REQ = 0x30;
        //public const int FIND_ACTOR_ID_REQ = 0x40;
        //public const int FIND_HOST_ID_REQ = 0x41;
        public const int ADD_HOST_ID_REQ = 0x41;
        public const int ADD_CLIENT_HOST_ID_REQ = 0x42;
        public const int ADD_ACTOR_ID_REQ = 0x43;
        public const int ADD_CLIENT_ACTOR_ID_REQ = 0x44;
        public const int REMOVE_HOST_ID_REQ = 0x45;
        public const int REMOVE_CLIENT_HOST_ID_REQ = 0x46;
        public const int REMOVE_ACTOR_ID_REQ = 0x47;
        public const int ON_ADD_HOST_ID_REQ = 0x48;
        public const int ON_ADD_CLIENT_HOST_ID_REQ = 0x49;
        public const int ON_ADD_ACTOR_ID_REQ = 0x50;
        public const int ON_ADD_CLIENT_ACTOR_ID_REQ = 0x51;
        public const int ON_REMOVE_HOST_ID_REQ = 0x52;
        public const int ON_REMOVE_CLIENT_HOST_ID_REQ = 0x53;
        public const int ON_REMOVE_ACTOR_ID_REQ = 0x54;
        public const int GET_ID_ALL_REQ = 0x55;



        public const int CALL_ACTOR_METHOD = 0xff;
    }

    public enum DisconnectReason
    {
        DEFAULT = 0x1,
        KICKED = 0x2,
        SERVER_ACTOR_DESTROY = 0x3,
    }
}