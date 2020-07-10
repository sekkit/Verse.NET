using System;

namespace Fenix.Common
{ 
    
    public enum Protocol : byte
    {
        HEARTBEAT = 0x1, 
        SPAWN_ACTOR = 0x5,
        MIGRATE_ACTOR = 0x6,
        CALL_ACTOR_METHOD = 0x1f
    }
}