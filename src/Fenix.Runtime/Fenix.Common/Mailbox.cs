//

using System;
using MessagePack;

namespace Fenix
{ 
    [MessagePackObject]
    public class Mailbox
    {
        [Key(0)]
        public ulong Id { get; set; }
        
        [Key(1)]
        public ulong ProtocolId { get; set; }
        
        [Key(2)]
        public byte[] Payload { get; set; }
    }
}