using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    [MessagePackObject]
    public class HostInfo
    {
        [Key(0)]
        public uint HostId { get; set; }

        [Key(1)]
        public string HostName { get; set; }

        [Key(2)]
        public string HostAddr { get; set; }
        
        [Key(3)]
        public Dictionary<uint, string> ServiceId2Name { get; set; }

        [Key(4)]
        public Dictionary<uint, string> ServiceId2TName { get; set; }

        //[Key(0)]
        //public Dictionary<uint, string> HID2ADDR = new Dictionary<uint, string>();

        //[Key(1)]
        //public Dictionary<string, uint> ADDR2HID = new Dictionary<string, uint>();

        //[Key(2)]
        //public Dictionary<uint, uint> AID2HID = new Dictionary<uint, uint>();

        //[Key(3)]
        //public Dictionary<uint, List<uint>> HID2AID = new Dictionary<uint, List<uint>>();

        //[Key(4)]
        //public Dictionary<string, uint> HNAME2HID = new Dictionary<string, uint>();

        //[Key(5)]
        //public Dictionary<uint, string> HID2HNAME = new Dictionary<uint, string>();

        //[Key(6)]
        //public Dictionary<string, uint> ANAME2AID = new Dictionary<string, uint>();

        //[Key(7)]
        //public Dictionary<uint, string> AID2ANAME = new Dictionary<uint, string>();

        //[Key(8)]
        //public Dictionary<uint, string> AID2TNAME = new Dictionary<uint, string>();
    }
}
