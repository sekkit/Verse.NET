using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Module.Shared
{
    public static class MathHelper
    {
        public static bool RandomByChance(float percent, int seed)
        {
            var rnd = new Random(seed);
            double v = rnd.NextDouble();
            if (v <= percent)
                return true;
            return false;
        }

        public static ulong GenLongID()
        {
            return NewUid(Guid.NewGuid());
            // ulong newID;
            // while ((newID = NewUid(Guid.NewGuid())) > ulong.MaxValue / 2) ;
            // return (Int64)newID;
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

        public static byte[] GenMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return hashBytes;
            }
        }
        
        public static ulong GenID64FromName(string name)
        {
            if(name.StartsWith("U") && name.Length == 20)
            {
                //UID
                return ulong.Parse(name.Substring(1));
            }

            var src = GenMd5(name);

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
        
        public static string ToRomanNumber(int number)
        {
            StringBuilder result = new StringBuilder();
            int[] digitsValues = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
            string[] romanDigits = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
            while (number > 0)
            {
                for (int i = digitsValues.Count() - 1; i >= 0; i--)
                    if (number / digitsValues[i] >= 1)
                    {
                        number -= digitsValues[i];
                        result.Append(romanDigits[i]);
                        break;
                    }
            }
            return result.ToString();
        }
        
        public static string ToMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash).ToString();
            }
        }
    } 

}