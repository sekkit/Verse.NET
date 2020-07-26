using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class Timer : IDisposable
    {
        public ulong Tid { get; set; }

        public long Delay { get; set; }
        public long Interval { get; set; }
        public bool Repeated { get; set; }

        public Action call { get; set; }

        private long fromTime { get; set; }


        /// <summary>
        /// Timer in MiliSeconds
        /// </summary>
        /// <param name="delay">MS</param>
        /// <param name="interval">MS</param>
        /// <param name="call">Method</param>
        /// <returns></returns>
        public static Timer Create(long delay, long interval, bool repeated, Action call)
        {
            var obj = new Timer();
            obj.Tid = Basic.GenID64();
            obj.fromTime = TimeUtil.GetTimeStampMS();
            obj.Delay = delay;
            obj.Repeated = repeated;
            obj.Interval = interval;
            obj.call = call;

            return obj;
        }

        public bool CheckTimeout(long curTime)
        {
            if (call == null)
                return true;
            
            if (curTime - this.fromTime >= Interval)
            {
                this.call();

                if(Repeated)
                {
                    this.fromTime = curTime;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            this.call = null;
        } 
    }
}
