using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Utils
{
    public class NetUtil
    {
        public static UInt64 GenLongID()
        {
            //ulong newID;
            //while ((newID = NewUid(Guid.NewGuid())) > ulong.MaxValue / 2);
            return NewUid(Guid.NewGuid());
        }

        static ulong NewUid(Guid guid)
        {
            var src = guid.ToByteArray();
            var buffer = new byte[8];
            Array.Copy(src, 0, buffer, 0, 8);
            ulong long1 = BitConverter.ToUInt64(buffer, 0);
            Array.Copy(src, 8, buffer, 0, 8);
            ulong long2 = BitConverter.ToUInt64(buffer, 0);

            ulong m = 0xc6a4a7935bd1e995;
            ulong h = m >> 4;
            const int r = 47;

            ulong k = long1;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            k = long2;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            h ^= h >> r;
            h *= m;
            h ^= h >> r;

            return h;
        }
    }
}
