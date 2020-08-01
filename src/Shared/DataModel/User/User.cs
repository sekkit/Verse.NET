using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shared.DataModel
{
    public class UserAttr : IMessage
    {
        [Key(0)]
        public int Coin;

        [Key(1)]
        public int BlueCoin;

        [Key(2)]
        public int RedCoin;

        [Key(3)]
        public int Strength;

        [Key(4)]
        public int Spirit;

        [Key(5)]
        public int Power;
    }

    [MessagePackObject]
    public class User : IMessage
    {
        //[Key(0)]
        //public UserAttr Attr;

        [Key(5)]
        public int Exp;

        [Key(10)]
        public int Coin;

        [Key(11)]
        public int BlueCoin;

        [Key(12)]
        public int RedCoin;

        [Key(13)]
        public int Strength;

        [Key(14)]
        public int Spirit;

        [Key(15)]
        public int Power;

    }
}
