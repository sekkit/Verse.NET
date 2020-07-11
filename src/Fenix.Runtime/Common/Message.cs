//

using System;
using Fenix.Common;
using MessagePack;

namespace Fenix
{ 
    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public ulong Id { get; set; }
        
        [Key(1)]
        public uint ProtocolId { get; set; }
        
        [Key(2)]
        public byte[] Payload { get; set; }
    }
}