using System;
using DotNetty.Common.Utilities;

namespace DotNetty.KCP.Base
{
    public class KcpUntils
    {
        public static long currentMs()
        {
            return DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
        }
    }
}