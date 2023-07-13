using System;
using Module.Helper;

namespace Module.Shared
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
            obj.Tid = MathHelper.GenLongID();
            obj.fromTime = TimeHelper.GetTimeStampMS();
            obj.Delay = delay;
            obj.Repeated = repeated;
            obj.Interval = interval;
            obj.call = call;

            return obj;
        }

        public bool Cancel()
        {
            if (Repeated)
                Repeated = false;

            this.fromTime = 0;
            this.call = null;
            
            return true;
        }
        
        public bool CheckDoneOrTimeout(long curTime)
        {
            if (call == null)
                return true;
            
            if (curTime - this.fromTime >= Interval)
            {
                MainThreadSynchronizationContext.Instance.Post(this.call);
                //this.call();

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