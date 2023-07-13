using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Helper
{
    public class TimeHelper
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

        public static DateTime ToDateTime(long ts)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            var dt = startTime.AddSeconds(ts);
            return dt;
        }
    }
}
