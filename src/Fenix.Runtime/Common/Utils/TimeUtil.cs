using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Utils
{
    public class TimeUtil
    {
        public static long GetTimeStamp()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds;
            return timeStamp;
        }

        public static long GetTimeStampMS()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds;
            return timeStamp;
        }

        public static double GetTimeStampMS2()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (DateTime.Now - startTime).TotalMilliseconds; 
        }

        //public static ulong GetTimeStamp()
        //{
        //    TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    return Convert.ToUInt64(ts.TotalSeconds * 1000);
        //}

        public static DateTime ToDateTime(long ts)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            var dt = startTime.AddSeconds(ts);
            return dt;
        }
    }
}
